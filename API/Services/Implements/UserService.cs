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
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Identity;
using Persistence.Helpers;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class UserService : IUserService
{
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IRepositoryBase<Team> _teamRepository;
    private readonly IRepositoryBase<TeamMember> _teamMemberRepository;
    private readonly IRepositoryBase<Company> _companyRepository;
    private readonly IRepositoryBase<CompanyMember> _companyMemberRepository;
    private readonly IFirebaseStorageService _firebaseStorageService;
    private readonly IMapper _mapper;

    public UserService(IRepositoryBase<User> userRepository, IRepositoryBase<Team> teamRepository, IRepositoryBase<TeamMember> teamMemberRepository, IRepositoryBase<Company> companyRepository, IRepositoryBase<CompanyMember> companyMemberRepository, IFirebaseStorageService firebaseStorageService, IMapper mapper)
    {
        _userRepository = userRepository;
        _teamRepository = teamRepository;
        _teamMemberRepository = teamMemberRepository;
        _companyRepository = companyRepository;
        _companyMemberRepository = companyMemberRepository;
        _firebaseStorageService = firebaseStorageService;
        _mapper = mapper;
    }

    public async Task<User> Create(CreateUserRequest model)
    {
        User entity = _mapper.Map(model, new User());
        var passwordHasher = new PasswordHasher<User>();
        entity.Password = passwordHasher.HashPassword(entity, model.Password);
        entity.IsActive = true;
        await _userRepository.CreateAsync(entity);
        return entity;
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
        var result = await _userRepository.FoundOrThrow(u => u.Id.Equals(id), new KeyNotFoundException("User is not exist"));
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

    public async Task Remove(int id)
    {
        var target = await _userRepository.FoundOrThrow(c => c.Id.Equals(id), new KeyNotFoundException("User is not exist"));
        await _userRepository.SoftDeleteAsync(target);
    }

    public async Task<User> Update(int id, UpdateUserRequest model)
    {
        var target = await _userRepository.FoundOrThrow(c => c.Id.Equals(id), new KeyNotFoundException("User is not exist"));
        User user = _mapper.Map(model, target);
        await _userRepository.UpdateAsync(user);
        return user;
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

        var linkImage = await _firebaseStorageService.UploadFirebaseAsync(stream, file.FileName);
        user.AvatarUrl = linkImage;
        await _userRepository.UpdateAsync(user);
        return linkImage;
    }


    private async Task GetTeamsOfUser(User user, GetUserProfileResponse entity)
    {
        var teamMember = await _teamMemberRepository.FirstOrDefaultAsync(x => x.MemberId == user.Id);

        if (teamMember != null)
        {
            var team = await _teamRepository.FirstOrDefaultAsync(x => x.Id == teamMember.TeamId, new string[] { "Manager" });
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
            var company = await _companyRepository.FirstOrDefaultAsync(x => x.Id == companyMember.CompanyId, new string[] { "CustomerAdmin" });
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
        string roleName = DataResponse.GetEnumDescription(user.Role);
        string fullname = $"{user.FirstName} {user.LastName}";
        FirestoreDb db = FirestoreDb.Create("itsds-v1");
        DocumentReference docRef = db.Collection("users").Document(user.Id.ToString());

        // Create a data object for the document
        Dictionary<string, object> data = new()
        {
            { "id", user.Id },
            { "name", fullname },
            { "username", user.Username! },
            { "image", user.AvatarUrl! },
            { "created_at", createdAtTime },
            { "modified_at", "" },
            { "last_active", lastActiveTime },
            { "role", roleName },
            { "about", $"I am {roleName}" },
            { "push_token", "" },
        };
        await docRef.SetAsync(data);
    }

    public async Task UpdateUserDocument(User user)
    {
        string createdAtTime = new DateTimeOffset((DateTime)user.CreatedAt!).ToUnixTimeMilliseconds().ToString();
        string lastActiveTime = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds().ToString();
        string roleName = DataResponse.GetEnumDescription(user.Role);
        string fullname = $"{user.FirstName} {user.LastName}";
        FirestoreDb db = FirestoreDb.Create("itsds-v1");
        DocumentReference docRef = db.Collection("users").Document(user.Id.ToString());

        // Create a data object for the document
        Dictionary<string, object> data = new()
        {
            { "id", user.Id },
            { "name", fullname },
            { "username", user.Username! },
            { "image", user.AvatarUrl! },
            { "created_at", createdAtTime },
            { "modified_at", "" },
            { "last_active", lastActiveTime },
            { "role", roleName },
            { "about", $"I am {roleName}" },
            { "push_token", "" },
        };
        await docRef.SetAsync(data);
    }
}
