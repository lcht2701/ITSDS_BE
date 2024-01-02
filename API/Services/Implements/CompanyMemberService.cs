using API.DTOs.Requests.CompanyMembers;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Constants.Enums;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Contracts;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class CompanyMemberService : ICompanyMemberService
{
    private readonly IRepositoryBase<CompanyMember> _companyMemberRepository;
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IMapper _mapper;

    public CompanyMemberService(IRepositoryBase<CompanyMember> companyMemberRepository, IRepositoryBase<User> userRepository, IMapper mapper)
    {
        _companyMemberRepository = companyMemberRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<List<CompanyMember>> Get()
    {
        var companyMembers = await _companyMemberRepository.ToListAsync();
        return companyMembers;
    }

    public async Task<List<CompanyMember>> GetCompanyAdmins(int companyId)
    {
        var result = (await _companyMemberRepository.WhereAsync(x => x.CompanyId.Equals(companyId) && x.IsCompanyAdmin)).ToList() 
            ?? throw new KeyNotFoundException("Company is not exist");
        return result;
    }

    public async Task<List<User>> GetMemberNotInCompany(int companyId)
    {
        var companyMemberIds = (await _companyMemberRepository.WhereAsync(x => x.CompanyId == companyId)).Select(x => x.MemberId).ToList() 
            ?? throw new KeyNotFoundException("Company is not exist");
        var users = (await _userRepository.WhereAsync(x => 
                    !companyMemberIds.Contains(x.Id) && 
                    x.Role == Role.Customer))
                    .ToList();
        return users;
    }

    public async Task<CompanyMember> GetById(int id)
    {
        var companyMember = await _companyMemberRepository.FirstOrDefaultAsync(x => x.Id.Equals(id)) ?? throw new KeyNotFoundException("Member is not exist");
        return companyMember;
    }

    public async Task<CompanyMember> Add(AddCompanyMemberRequest model, int currentUserId)
    {
        CompanyMember currentUserMember = await IsCompanyAdmin(currentUserId);
        var userAccount = _mapper.Map(model.User, new User());
        var userResult = await _userRepository.CreateAsync(userAccount);
        var member = new CompanyMember()
        {
            MemberId = userResult.Id,
            CompanyId = currentUserMember.CompanyId,
            IsCompanyAdmin = model.IsCompanyAdmin,
            MemberPosition = model.MemberPosition
        };
        await _companyMemberRepository.CreateAsync(member);
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
        await _companyMemberRepository.DeleteAsync(companyMember);
    }

    private async Task<CompanyMember> IsCompanyAdmin(int currentUserId)
    {
        var currentUser = await _userRepository.FoundOrThrow(x => x.Id.Equals(currentUserId), new KeyNotFoundException("User is not found"));
        var currentUserMember = await _companyMemberRepository.FirstOrDefaultAsync(x => x.MemberId.Equals(currentUser.Id));
        if (currentUserMember == null || !currentUserMember.IsCompanyAdmin)
        {
            throw new UnauthorizedException("User is not authorize for this action");
        }

        return currentUserMember;
    }
}
