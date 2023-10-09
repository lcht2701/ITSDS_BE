using API.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Persistence.Context;
using Persistence.Repositories.Interfaces;
using Persistence.Repositories;
using System.Text;
using API.Configurations;
using System.Reflection;
using Persistence.Services.Interfaces;
using Persistence.Services.Implements;
using Domain.Application.AppConfig;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var CorsPolicy = "CorsPolicy";

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

builder.Services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
builder.Services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IFirebaseStorageService, FirebaseStorageService>();
builder.Services.AddSingleton<FirebaseStorageService>();

builder.Services.AddControllers(options => options.Filters.Add<ValidateModelStateFilter>());
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = int.MaxValue;
});

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
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
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

app.MapControllers();

app.Run();
