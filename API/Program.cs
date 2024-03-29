using API.Configurations;
using API.Services.Implements;
using API.Services.Interfaces;
using API.Utils;
using Domain.Entities.Mails;
using FirebaseAdmin;
using FluentValidation.AspNetCore;
using Google.Apis.Auth.OAuth2;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Persistence.Context;
using Persistence.Helpers.Caching;
using Persistence.Repositories;
using Persistence.Repositories.Interfaces;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var CorsPolicy = "CorsPolicy";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection")));
builder.Services.AddHangfireServer();

// Add services to the container.
builder.Services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));
builder.Services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
builder.Services.AddScoped<IAssignmentService, AssignmentService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<ICompanyAddressService, CompanyAddressService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IModeService, ModeService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<ITeamMemberService, TeamMemberService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<ITicketSolutionService, TicketSolutionService>();
builder.Services.AddScoped<ITicketTaskService, TicketTaskService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<ICompanyMemberService, CompanyMemberService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IServiceContractService, ServiceContractService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IMessagingService, MessagingService>();
builder.Services.AddScoped<IFirebaseService, FirebaseService>();
builder.Services.AddScoped<IHangfireJobService, HangfireJobService>();
builder.Services.AddScoped<IAttachmentService, AttachmentService>();
builder.Services.AddScoped<IReactionService, ReactionService>();
builder.Services.AddScoped<IExportService, ExportService>();

builder.Services.AddSingleton<ICacheService, CacheService>();

builder.Services.AddControllers(options => options.Filters.Add<ValidateModelStateFilter>())
    .AddFluentValidation(c => c.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<FormOptions>(options => { options.MultipartBodyLengthLimit = int.MaxValue; });

builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ITSDS",
        Version = "v1"
    });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Please enter a valid token",
        BearerFormat = "JWT",
        Scheme = "Bearer",
        Type = SecuritySchemeType.Http,
        In = ParameterLocation.Header,
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// Add Authentication and JWTBearer
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy,
        policy =>
        {
            policy.WithOrigins("*")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "admin_sdk.json")),
    ProjectId = "itsds-v1"
});

Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",
    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "admin_sdk.json"));

builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
await app.Services.ApplyMigrations();

app.UseHttpsRedirection();
app.UseCors(CorsPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.UseAutoWrapper();
app.UseHangfireDashboard("/hangfire");
app.MapControllers();
app.MapHangfireDashboard();

//Running Background Services
RecurringJob.AddOrUpdate<IHangfireJobService>("remove-old-device-token", x => x.RemoveOldToken(30),
    Cron.Daily());
RecurringJob.AddOrUpdate<IHangfireJobService>("lock-overdue-payment-of-contract", x => x.UpdateStatusOfOverduePaymentContract(0),
   Cron.Daily());
RecurringJob.AddOrUpdate<IHangfireJobService>("notify-near-end-payment", x => x.NotifyNearEndPayment(3),
   Cron.Daily());
RecurringJob.AddOrUpdate<IHangfireJobService>("ticket-summary", x => x.TicketSummaryNotificationJob(),
    Cron.Daily(8, 0));
RecurringJob.AddOrUpdate<IHangfireJobService>("update-contract-status", x => x.UpdateStatusOfContract(), "*/5 * * * *");
RecurringJob.AddOrUpdate<IHangfireJobService>("notify-near-expired-contracts", x => x.NotifyNearExpiredContract(),
    Cron.Daily(8, 0));

app.Run();