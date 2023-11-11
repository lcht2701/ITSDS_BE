using API.DTOs.Requests.Companies;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Models.Contracts;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class CompanyService : ICompanyService
{
    private readonly IRepositoryBase<Company> _companyRepository;
    private readonly IMapper _mapper;

    public CompanyService(IRepositoryBase<Company> companyRepository, IMapper mapper)
    {
        _companyRepository = companyRepository;
        _mapper = mapper;
    }

    public async Task<List<Company>> Get()
    {
        return await _companyRepository.ToListAsync();
    }

    public async Task<Company> GetById(int id)
    {
        var company = await _companyRepository.FoundOrThrow(x => x.Id.Equals(id), new KeyNotFoundException("Company is not exist"));
        return company;
    }

    public async Task<Company> Create(CreateCompanyRequest model)
    {
        var entity = _mapper.Map(model, new Company());
        await _companyRepository.CreateAsync(entity);
        return entity;
    }

    public async Task<Company> Update(int id, UpdateCompanyRequest model)
    {
        var target = await _companyRepository.FoundOrThrow(c => c.Id.Equals(id), new KeyNotFoundException("Company is not exist"));
        Company entity = _mapper.Map(model, target);
        await _companyRepository.UpdateAsync(entity);
        return target;
    }

    public async Task Remove(int id)
    {
        var target = await _companyRepository.FoundOrThrow(c => c.Id.Equals(id), new KeyNotFoundException("Team is not exist"));
        await _companyRepository.SoftDeleteAsync(target);
    }

}
