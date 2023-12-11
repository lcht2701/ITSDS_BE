using API.DTOs.Requests.CompanyMembers;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Constants.Enums;
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

    public async Task<List<User>> GetMemberNotInCompany(int companyId)
    {
        var companyMemberIds = (await _companyMemberRepository.WhereAsync(x => x.CompanyId == companyId)).Select(x => x.MemberId).ToList() ?? throw new KeyNotFoundException("Company is not exist");
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
    public async Task<CompanyMember> Add(AddCompanyMemberRequest model)
    {
        var member = _mapper.Map<CompanyMember>(model);
        await _companyMemberRepository.CreateAsync(member);
        return member;
    }

    public async Task<CompanyMember> Update(int id, UpdateCompanyMemberRequest model)
    {
        var companyMember = await _companyMemberRepository.FirstOrDefaultAsync(x => x.Id.Equals(id)) ?? throw new KeyNotFoundException("Member is not exist");
        var entity = _mapper.Map(model, companyMember);
        await _companyMemberRepository.UpdateAsync(entity);
        return entity;
    }

    public async Task Remove(int id)
    {
        var companyMember = await _companyMemberRepository.FirstOrDefaultAsync(x => x.Id.Equals(id)) ?? throw new KeyNotFoundException("Member is not exist");
        await _companyMemberRepository.DeleteAsync(companyMember);
    }

}
