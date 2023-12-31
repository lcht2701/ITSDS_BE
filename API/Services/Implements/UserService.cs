using API.DTOs.Requests.Users;
using API.DTOs.Responses.Companies;
using API.DTOs.Responses.Teams;
using API.DTOs.Responses.Users;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Application.AppConfig;
using Domain.Constants.Enums;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Contracts;
using Domain.Models.Tickets;
using Google.Cloud.Firestore;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;
using Persistence.Helpers;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class UserService : IUserService
{
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IRepositoryBase<Team> _teamRepository;
    private readonly IRepositoryBase<Company> _companyRepository;
    private readonly IRepositoryBase<TeamMember> _teamMemberRepository;
    private readonly IRepositoryBase<CompanyMember> _companyMemberRepository;
    private readonly IFirebaseService _firebaseService;
    private readonly IMapper _mapper;
    private readonly MailSettings _mailSettings;

    public UserService(IRepositoryBase<User> userRepository, IRepositoryBase<Team> teamRepository,
        IRepositoryBase<TeamMember> teamMemberRepository, IRepositoryBase<Company> companyRepository,
        IRepositoryBase<CompanyMember> companyMemberRepository, IFirebaseService firebaseService, IMapper mapper,
        IOptions<MailSettings> mailSettings)
    {
        _userRepository = userRepository;
        _teamRepository = teamRepository;
        _teamMemberRepository = teamMemberRepository;
        _companyRepository = companyRepository;
        _companyMemberRepository = companyMemberRepository;
        _firebaseService = firebaseService;
        _mapper = mapper;
        _mailSettings = mailSettings.Value;
    }

    public async Task<List<GetUserResponse>> Get()
    {
        var result = await _userRepository.ToListAsync();
        var response = new List<GetUserResponse>();
        foreach (var user in result)
        {
            var entity = _mapper.Map(user, new GetUserResponse());
            DataResponse.CleanNullableDateTime(entity);
            response.Add(entity);
        }

        return response;
    }

    public async Task<GetUserResponse> GetById(int id)
    {
        var result =
            await _userRepository.FoundOrThrow(u => u.Id.Equals(id), new KeyNotFoundException("User is not exist"));
        var entity = _mapper.Map(result, new GetUserResponse());
        DataResponse.CleanNullableDateTime(entity);
        return entity;
    }

    public async Task<GetUserProfileResponse> GetProfile(int userId)
    {
        var user = await _userRepository.FoundOrThrow(u => u.Id.Equals(userId),
            new KeyNotFoundException("User is not exist"));
        var entity = _mapper.Map(user, new GetUserProfileResponse());
        DataResponse.CleanNullableDateTime(entity);
        if (user.Role == Role.Customer)
        {
            await GetCompanyOfUser(user, entity);
        }
        else
        {
            await GetTeamsOfUser(user, entity);
        }

        return entity;
    }

    public async Task<User> Create(CreateUserOrCustomerAdmin model)
    {
        User entity = _mapper.Map(model.UserModel, new User());
        var passwordHasher = new PasswordHasher<User>();
        entity.Password = passwordHasher.HashPassword(entity, model.UserModel.Password);
        //Default when create new account
        entity.IsActive = true;
        var result = await _userRepository.CreateAsync(entity);
        if (model.CompanyId != null)
        {
            await _companyMemberRepository.CreateAsync(new CompanyMember()
            {
                MemberId = result.Id,
                CompanyId = (int)model.CompanyId,
                IsCompanyAdmin = model.isCompanyAdmin
            });
        }
        await SendUserCreatedNotification(model.UserModel);
        return entity;
    }

    public async Task<User> Update(int id, UpdateUserRequest model)
    {
        var target =
            await _userRepository.FoundOrThrow(c => c.Id.Equals(id), new KeyNotFoundException("User is not exist"));
        User user = _mapper.Map(model, target);
        await _userRepository.UpdateAsync(user);
        return user;
    }

    public async Task Remove(int id)
    {
        var target =
            await _userRepository.FoundOrThrow(c => c.Id.Equals(id), new KeyNotFoundException("User is not exist"));
        await _userRepository.SoftDeleteAsync(target);
        #region Remove in Company Member
        var companyMember = await _companyMemberRepository.WhereAsync(x => x.MemberId == target.Id);
        foreach (var member in companyMember) await _companyMemberRepository.DeleteAsync(member);
        #endregion
        #region Remove In Team Member
        var teamMember = await _teamMemberRepository.WhereAsync(x => x.MemberId == target.Id);
        foreach (var member in teamMember) await _teamMemberRepository.DeleteAsync(member);
        #endregion
    }

    public async Task<User> UpdateProfile(int id, UpdateProfileRequest model)
    {
        var target = await _userRepository.FoundOrThrow(c => c.Id.Equals(id),
            new NotFoundException("User is not found"));
        User user = _mapper.Map(model, target);
        await _userRepository.UpdateAsync(user);
        return user;
    }

    public async Task<User> UploadAvatarByUrl(int userId, UpdateAvatarUrlRequest model)
    {
        var user = await _userRepository.FoundOrThrow(c => c.Id.Equals(userId),
            new KeyNotFoundException("User is not found"));
        user.AvatarUrl = model.AvatarUrl;
        await _userRepository.UpdateAsync(user);
        return user;
    }

    public async Task<string> UploadImageFirebase(int userId, IFormFile file)
    {
        var user = await _userRepository.FoundOrThrow(c => c.Id.Equals(userId),
            new KeyNotFoundException("User is not exist"));
        if (file == null || file.Length == 0)
        {
            throw new BadRequestException("No file uploaded.");
        }

        var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Position = 0;

        var linkImage = await _firebaseService.UploadFirebaseAsync(stream, file.FileName);
        user.AvatarUrl = linkImage;
        await _userRepository.UpdateAsync(user);
        await UpdateUserDocument(user);
        return linkImage;
    }

    private async Task GetTeamsOfUser(User user, GetUserProfileResponse entity)
    {
        var teamMember = await _teamMemberRepository.FirstOrDefaultAsync(x => x.MemberId == user.Id);

        if (teamMember != null)
        {
            var team = await _teamRepository.FirstOrDefaultAsync(x => x.Id == teamMember.TeamId,
                new string[] { "Manager" });
            if (team != null)
            {
                entity.Team = _mapper.Map(team, new GetTeamResponse());
            }
        }
    }

    private async Task GetCompanyOfUser(User user, GetUserProfileResponse entity)
    {
        var companyMember = await _companyMemberRepository.FirstOrDefaultAsync(x => x.MemberId == user.Id);

        if (companyMember != null)
        {
            var company = await _companyRepository.FirstOrDefaultAsync(x => x.Id == companyMember.CompanyId,
                new string[] { "CustomerAdmin" });
            if (company != null)
            {
                entity.Company = _mapper.Map(company, new GetCompanyResponse());
            }
        }
    }

    public async Task CreateUserDocument(User user)
    {
        string createdAtTime = new DateTimeOffset((DateTime)user.CreatedAt!).ToUnixTimeMilliseconds().ToString();
        string lastActiveTime = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds().ToString();
        string about = $"I am {DataResponse.GetEnumDescription(user.Role)}";
        string fullname = $"{user.FirstName} {user.LastName}";
        FirestoreDb db = FirestoreDb.Create("itsds-v1");
        DocumentReference docRef = db.Collection("users").Document(user.Id.ToString());

        // Create a data object for the document
        Dictionary<string, object> data = new()
        {
            { "id", user.Id.ToString() ?? "" },
            { "name", fullname ?? "" },
            { "email", user.Email! ?? "" },
            { "image", user.AvatarUrl! ?? "" },
            { "created_at", createdAtTime ?? "" },
            { "last_active", lastActiveTime ?? "" },
            { "about", about ?? "" },
            { "is_active", true },
            { "push_token", "" },
        };
        await docRef.SetAsync(data);
    }

    public async Task UpdateUserDocument(User user)
    {
        FirestoreDb db = FirestoreDb.Create("itsds-v1");
        DocumentReference docRef = db.Collection("users").Document(user.Id.ToString());
        string newFullname = $"{user.FirstName} {user.LastName}";
        string newAbout = $"I am {DataResponse.GetEnumDescription(user.Role)}";

        // Get the existing user document data
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
        if (snapshot.Exists)
        {
            // Extract existing data
            Dictionary<string, object> existingData = snapshot.ToDictionary();

            // Update only the fields that need to be changed
            existingData["name"] = newFullname ?? existingData["name"];
            existingData["email"] = user.Username ?? existingData["email"];
            existingData["image"] = user.AvatarUrl ?? existingData["image"];
            existingData["about"] = newAbout ?? existingData["about"];

            // Update the Firestore document with the modified data
            await docRef.UpdateAsync(existingData);
        }
        else
        {
            // Handle the case where the user document doesn't exist
            // You can choose to create a new document or handle the error as needed.
        }
    }

    private async Task SendUserCreatedNotification(CreateUserRequest dto)
    {
        using (MimeMessage emailMessage = new MimeMessage())
        {
            string fullname = $"{dto.FirstName} {dto.LastName}";
            string roleName = DataResponse.GetEnumDescription(dto.Role);
            MailboxAddress emailFrom = new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail);
            emailMessage.From.Add(emailFrom);
            MailboxAddress emailTo = new MailboxAddress(fullname,
                dto.Email);
            emailMessage.To.Add(emailTo);

            emailMessage.Subject = "Welcome to ITSDS - Your New Account Details";
            string emailTemplateText = @"<!DOCTYPE html>
<html lang=""en"">
<head>
  <meta charset=""UTF-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
  <title>Welcome to ITSDS System!</title>
</head>
<body style=""font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; margin: 0; padding: 0; background-color: #f8f9fa;"">

  <div style=""max-width: 600px; margin: 20px auto; background-color: #ffffff; border-radius: 5px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);"">
    <div style=""background-color: #007bff; color: #fff; padding: 20px; text-align: center; border-top-left-radius: 5px; border-top-right-radius: 5px;"">
      <h1>Welcome to ITSDS System!</h1>
    </div>
    <div style=""padding: 20px;"">
      <p>Dear {0},</p>
      <p>We are delighted to welcome you to ITSDS! Your new account has been successfully created, and we're excited to have you on board. Below are your account details:</p>
      <ul style=""list-style-type: none; padding: 0;"">
        <li style=""margin-bottom: 10px;""><strong>Username:</strong> {1}</li>
        <li style=""margin-bottom: 10px;""><strong>Password:</strong> {2}</li>
        <li style=""margin-bottom: 10px;""><strong>Email:</strong> {3}</li>
        <li style=""margin-bottom: 10px;""><strong>Role:</strong> {4}</li>
      </ul>
      <p>Please keep this information secure, and do not share your password with anyone. If you have any questions or concerns regarding your account, feel free to contact our support team at <a href=""mailto:itsdskns@gmail.com"">itsdskns@gmail.com</a>.</p>
      <p>We recommend logging in at <a href=""https://dichvuit.hisoft.vn/"">https://dichvuit.hisoft.vn/</a> to update your password for added security.</p>
    </div>
    <div style=""background-color: #f1f1f1; padding: 10px; text-align: center; border-bottom-left-radius: 5px; border-bottom-right-radius: 5px;"">
      <p>Best regards, ITSDS</p>
    </div>
  </div>

</body>
</html>
";
            emailTemplateText = string.Format(emailTemplateText, fullname, dto.Username, dto.Password, dto.Email,
                roleName);

            BodyBuilder emailBodyBuilder = new BodyBuilder();
            emailBodyBuilder.HtmlBody = emailTemplateText;
            emailBodyBuilder.TextBody = "Plain Text goes here to avoid marked as spam for some email servers.";

            emailMessage.Body = emailBodyBuilder.ToMessageBody();

            using (SmtpClient mailClient = new SmtpClient())
            {
                await mailClient.ConnectAsync(_mailSettings.Server, _mailSettings.Port,
                    SecureSocketOptions.StartTls);
                await mailClient.AuthenticateAsync(_mailSettings.SenderEmail, _mailSettings.Password);
                await mailClient.SendAsync(emailMessage);
                await mailClient.DisconnectAsync(true);
            }
        }
    }

    #region Selection List By Roles

    public async Task<List<User>> GetManagers()
    {
        return (await _userRepository.WhereAsync(x => x.Role.Equals(Role.Manager))).ToList();
    }

    public async Task<List<User>> GetTechnicians()
    {
        return (await _userRepository.WhereAsync(x => x.Role.Equals(Role.Technician))).ToList();
    }

    public async Task<List<User>> GetCustomers()
    {
        return (await _userRepository.WhereAsync(x => x.Role.Equals(Role.Customer))).ToList();
    }

    public async Task<List<User>> GetAdmins()
    {
        return (await _userRepository.WhereAsync(x => x.Role.Equals(Role.Admin))).ToList();
    }

    public async Task<List<User>> GetAccountants()
    {
        return (await _userRepository.WhereAsync(x => x.Role.Equals(Role.Accountant))).ToList();
    }

    #endregion
}