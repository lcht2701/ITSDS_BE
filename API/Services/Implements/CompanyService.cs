using API.DTOs.Requests.Companies;
using API.DTOs.Responses.Companies;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Models.Contracts;
using Persistence.Helpers.Caching;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class CompanyService : ICompanyService
{
    private readonly IRepositoryBase<Company> _companyRepository;
    private readonly IRepositoryBase<CompanyAddress> _companyAddressRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;

    public CompanyService(IRepositoryBase<Company> companyRepository, IRepositoryBase<CompanyAddress> companyAddressRepository, ICacheService cacheService, IMapper mapper)
    {
        _companyRepository = companyRepository;
        _companyAddressRepository = companyAddressRepository;
        _cacheService = cacheService;
        _mapper = mapper;
    }

    public async Task<List<GetCompanyResponse>> Get()
    {
        var cacheData = _cacheService.GetData<List<GetCompanyResponse>>("companies");
        if (cacheData == null || !cacheData.Any())
        {
            var companyList = await _companyRepository.ToListAsync();
            cacheData = _mapper.Map(companyList, new List<GetCompanyResponse>());
            foreach (var data in cacheData)
            {
                data.Addresses = (await _companyAddressRepository.WhereAsync(x => x.CompanyId == data.Id)).ToList();
            }
            var expiryTime = DateTimeOffset.Now.AddSeconds(30);
            _cacheService.SetData("companies", cacheData, expiryTime);
        }
        return cacheData;
    }

    public async Task<GetCompanyResponse> GetById(int id)
    {
        var cacheData = _cacheService.GetData<GetCompanyResponse>($"company-{id}");
        if (cacheData == null)
        {
            var company = await _companyRepository.FoundOrThrow(x => x.Id.Equals(id), new KeyNotFoundException("Company is not exist"));
            cacheData = _mapper.Map(company, new GetCompanyResponse());
            cacheData.Addresses = (await _companyAddressRepository.WhereAsync(x => x.CompanyId == company.Id)).ToList();
            var expiryTime = DateTimeOffset.Now.AddSeconds(30);
            _cacheService.SetData($"company-{cacheData.Id}", cacheData, expiryTime);
        }
        return cacheData;
    }

    public async Task<Company> Create(CreateCompanyRequest model)
    {
        var entity = _mapper.Map(model, new Company());
        var result = await _companyRepository.CreateAsync(entity);
        #region Cache
        var expiryTime = DateTimeOffset.Now.AddSeconds(30);
        _cacheService.SetData($"company-{result.Id}", result, expiryTime);
        var cacheList = await _companyRepository.ToListAsync();
        _cacheService.SetData("companies", cacheList, expiryTime);
        #endregion
        return result;
    }

    public async Task<Company> Update(int id, UpdateCompanyRequest model)
    {
        var target = await _companyRepository.FoundOrThrow(c => c.Id.Equals(id), new KeyNotFoundException("Company is not exist"));
        Company entity = _mapper.Map(model, target);
        var result = await _companyRepository.UpdateAsync(entity);
        #region Cache
        var expiryTime = DateTimeOffset.Now.AddSeconds(30);
        _cacheService.SetData($"company-{result.Id}", result, expiryTime);
        var cacheList = await _companyRepository.ToListAsync();
        _cacheService.SetData("companies", cacheList, expiryTime);
        #endregion
        return target;
    }

    public async Task Remove(int id)
    {
        var target = await _companyRepository.FoundOrThrow(c => c.Id.Equals(id), new KeyNotFoundException("Team is not exist"));
        await _companyRepository.SoftDeleteAsync(target);
        #region Cache
        var expiryTime = DateTimeOffset.Now.AddSeconds(30);
        _cacheService.RemoveData($"company-{target.Id}");
        var cacheList = await _companyRepository.ToListAsync();
        _cacheService.SetData("companies", cacheList, expiryTime);
        #endregion
    }
}
