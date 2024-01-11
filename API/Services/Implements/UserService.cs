using API.DTOs.Requests.Users;
using API.DTOs.Responses.Companies;
using API.DTOs.Responses.Teams;
using API.DTOs.Responses.Users;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Constants.Enums;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Contracts;
using Domain.Models.Tickets;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Persistence.Helpers;
using Persistence.Repositories.Interfaces;
using static Grpc.Core.Metadata;

namespace API.Services.Implements;

public class UserService : IUserService
{
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IRepositoryBase<Team> _teamRepository;
    private readonly IRepositoryBase<Company> _companyRepository;
    private readonly IRepositoryBase<TeamMember> _teamMemberRepository;
    private readonly IRepositoryBase<CompanyMember> _companyMemberRepository;
    private readonly IFirebaseService _firebaseService;
    private readonly IMailService _mailService;
    private readonly IMapper _mapper;

    public UserService(IRepositoryBase<User> userRepository, IRepositoryBase<Team> teamRepository, IRepositoryBase<Company> companyRepository, IRepositoryBase<TeamMember> teamMemberRepository, IRepositoryBase<CompanyMember> companyMemberRepository, IFirebaseService firebaseService, IMailService mailService, IMapper mapper)
    {
        _userRepository = userRepository;
        _teamRepository = teamRepository;
        _companyRepository = companyRepository;
        _teamMemberRepository = teamMemberRepository;
        _companyMemberRepository = companyMemberRepository;
        _firebaseService = firebaseService;
        _mailService = mailService;
        _mapper = mapper;
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

    public async Task<User> Create(CreateUserRequest model)
    {
        User entity = _mapper.Map(model, new User());
        var checkMailDuplicated = await _userRepository.FirstOrDefaultAsync(x => x.Email == entity.Email);
        if (checkMailDuplicated != null)
        {
            throw new BadRequestException("Email is exist. Please use a different email address to create user.");
        }
        var passwordHasher = new PasswordHasher<User>();
        var generatedPassword = CommonService.CreateRandomPassword();
        entity.Password = passwordHasher.HashPassword(entity, generatedPassword);
        //Default when create new account
        entity.IsActive = true;
        var result = await _userRepository.CreateAsync(entity);
        await _firebaseService.CreateFirebaseUser(model.Email, generatedPassword);
        await _firebaseService.CreateUserDocument(result);
        if (entity.Role == Role.Customer)
        {
            await _companyMemberRepository.CreateAsync(new CompanyMember()
            {
                MemberId = result.Id,
                CompanyId = model.CompanyDetail.CompanyId,
                IsCompanyAdmin = model.CompanyDetail.IsCompanyAdmin,
                CompanyAddressId = model.CompanyDetail.CompanyAddressId,
                MemberPosition = model.CompanyDetail.IsCompanyAdmin == true ? "Company Admin" : "Nhân viên"
            });
        }
        string fullname = $"{model.FirstName} {model.LastName}";
        string roleName = model.CompanyDetail.IsCompanyAdmin == true ? "Company Admin" : "Customer";
        BackgroundJob.Enqueue(() => _mailService.SendUserCreatedNotification(fullname, model.Username, model.Email, generatedPassword, roleName));
        return result;
    }

    public async Task<User> Update(int id, UpdateUserRequest model)
    {
        var checkMailDuplicated = await _userRepository.FirstOrDefaultAsync(x => x.Email == model.Email);
        if (checkMailDuplicated != null)
        {
            throw new BadRequestException("Email is exist. Please use a different email address to create user.");
        }
        var target =
            await _userRepository.FoundOrThrow(c => c.Id.Equals(id),
            new KeyNotFoundException("User is not exist"));
        await _firebaseService.UpdateFirebaseUser(target.Email, model.Email, null);
        User user = _mapper.Map(model, target);
        var result = await _userRepository.UpdateAsync(user);
        await _firebaseService.UpdateUserDocument(result);
        return user;
    }

    public async Task Remove(int id)
    {
        var target =
            await _userRepository.FoundOrThrow(c => c.Id.Equals(id), new KeyNotFoundException("User is not exist"));
        await _firebaseService.RemoveFirebaseAccount(id);
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
        var target =
            await _userRepository.FoundOrThrow(c => c.Id.Equals(id),
            new KeyNotFoundException("User is not exist"));
        await _firebaseService.UpdateFirebaseUser(target.Email, model.Email, null);
        User user = _mapper.Map(model, target);
        var result = await _userRepository.UpdateAsync(user);
        await _firebaseService.UpdateUserDocument(result);
        return user;
    }

    public async Task<User> UploadAvatarByUrl(int userId, UpdateAvatarUrlRequest model)
    {
        var user = await _userRepository.FoundOrThrow(c => c.Id.Equals(userId),
            new KeyNotFoundException("User is not found"));
        user.AvatarUrl = model.AvatarUrl;
        var result = await _userRepository.UpdateAsync(user);
        await _firebaseService.UpdateUserDocument(user);
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
        await _firebaseService.UpdateUserDocument(user);
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
            var company = await _companyRepository.FirstOrDefaultAsync(x => x.Id == companyMember.CompanyId);
            if (company != null)
            {
                entity.Company = _mapper.Map(company, new GetCompanyResponse());
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