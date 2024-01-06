using API.DTOs.Requests.CompanyMembers;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Application.AppConfig;
using Domain.Constants.Enums;
using Domain.Models;
using Domain.Models.Contracts;
using Hangfire;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;
using Persistence.Helpers;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class CompanyMemberService : ICompanyMemberService
{
    private readonly IRepositoryBase<CompanyMember> _companyMemberRepository;
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IFirebaseService _firebaseService;
    private readonly IMailService _mailService;
    private readonly IMapper _mapper;

    public CompanyMemberService(IRepositoryBase<CompanyMember> companyMemberRepository, IRepositoryBase<User> userRepository, IFirebaseService firebaseService, IMailService mailService, IMapper mapper)
    {
        _companyMemberRepository = companyMemberRepository;
        _userRepository = userRepository;
        _firebaseService = firebaseService;
        _mailService = mailService;
        _mapper = mapper;
    }

    public async Task<List<CompanyMember>> Get(int userId)
    {
        List<CompanyMember> result = new();
        var currentUser = await _userRepository.FirstOrDefaultAsync(x => x.Id == userId);
        var memberInfo = await _companyMemberRepository.FirstOrDefaultAsync(x => x.MemberId == userId);
        switch (currentUser.Role)
        {

            case Role.Customer:
                {
                    result = (await _companyMemberRepository.WhereAsync(x => x.CompanyId.Equals(memberInfo.CompanyId), new string[] { "Member", "Company" })).ToList();
                    break;
                }
            default:
                {
                    result = (await _companyMemberRepository.GetAsync(navigationProperties: new string[] { "Member", "Company" })).ToList();
                    break;
                }
        }
        return result;
    }

    public async Task<List<CompanyMember>> GetCompanyAdmins(int companyId)
    {
        var result = (await _companyMemberRepository.WhereAsync(x => x.CompanyId.Equals(companyId) && x.IsCompanyAdmin, new string[] { "Member", "Company" })).ToList()
            ?? throw new KeyNotFoundException("Company is not exist");
        return result;
    }

    public async Task<List<User>> GetMemberNotInCompany(int companyId)
    {
        var companyMemberIds = (await _companyMemberRepository.WhereAsync(x => x.CompanyId == companyId)).Select(x => x.MemberId).ToList()
            ?? throw new KeyNotFoundException("Company is not exist");
        var users = (await _userRepository.WhereAsync(x =>
                    !companyMemberIds.Contains(x.Id) &&
                    x.Role == Role.Customer, new string[] { "Member", "Company" }))
                    .ToList();
        return users;
    }

    public async Task<CompanyMember> GetById(int id)
    {
        var companyMember = await _companyMemberRepository.FirstOrDefaultAsync(x => x.Id.Equals(id), new string[] { "Member", "Company" }) ?? throw new KeyNotFoundException("Member is not exist");
        return companyMember;
    }

    public async Task<CompanyMember> Add(AddCompanyMemberRequest model, int currentUserId)
    {
        CompanyMember currentUserMember = await IsCompanyAdmin(currentUserId);
        var userAccount = _mapper.Map(model.User, new User());

        //Default when create new account
        userAccount.Role = Role.Customer;
        var generatedPassword = CommonService.CreateRandomPassword();
        var passwordHasher = new PasswordHasher<User>();
        userAccount.Password = passwordHasher.HashPassword(userAccount, generatedPassword);
        userAccount.IsActive = true;

        var userResult = await _userRepository.CreateAsync(userAccount);
        var member = new CompanyMember()
        {
            MemberId = userResult.Id,
            CompanyId = currentUserMember.CompanyId,
            IsCompanyAdmin = model.IsCompanyAdmin,
            MemberPosition = model.MemberPosition != null ? model.MemberPosition : "Nhân viên",
            CompanyAddressId = model.CompanyAddressId
        };
        await _companyMemberRepository.CreateAsync(member);
        string fullname = $"{model.User.FirstName} {model.User.LastName}";
        string roleName = model.IsCompanyAdmin == true ? "Company Admin" : "Customer";
        BackgroundJob.Enqueue(() => _mailService.SendUserCreatedNotification(fullname, model.User.Username, model.User.Email, generatedPassword, roleName));
        await _firebaseService.CreateFirebaseUser(model.User.Email, generatedPassword);
        await _firebaseService.CreateUserDocument(userResult);
        return member;
    }

    public async Task<CompanyMember> Update(int id, UpdateCompanyMemberRequest model, int currentUserId)
    {
        CompanyMember currentUserMember = await IsCompanyAdmin(currentUserId);
        var companyMember = await _companyMemberRepository.FirstOrDefaultAsync(x => x.Id.Equals(id)) ?? throw new KeyNotFoundException("Member is not exist");
        var entity = _mapper.Map(model, companyMember);
        await _companyMemberRepository.UpdateAsync(entity);
        return entity;
    }

    public async Task Remove(int id, int currentUserId)
    {
        CompanyMember currentUserMember = await IsCompanyAdmin(currentUserId);
        var companyMember = await _companyMemberRepository.FirstOrDefaultAsync(x => x.Id.Equals(id)) ?? throw new KeyNotFoundException("Member is not exist");
        var userAccount = await _userRepository.FirstOrDefaultAsync(x => x.Id == currentUserId);
        await _userRepository.DeleteAsync(userAccount);
        await _firebaseService.RemoveFirebaseAccount(userAccount.Id);
        await _companyMemberRepository.DeleteAsync(companyMember);
    }

    private async Task<CompanyMember> IsCompanyAdmin(int currentUserId)
    {
        var currentUserMember = await _companyMemberRepository
                                            .FirstOrDefaultAsync(x =>
                                                            x.MemberId.Equals(currentUserId) &&
                                                            x.IsCompanyAdmin == true);
        if (currentUserMember == null)
        {
            throw new UnauthorizedAccessException("User is not authorize for this action");
        }

        return currentUserMember;
    }
}
