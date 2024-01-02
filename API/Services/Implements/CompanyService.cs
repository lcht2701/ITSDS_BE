﻿using API.DTOs.Requests.Companies;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Models.Contracts;
using Domain.Models.Tickets;
using Persistence.Helpers.Caching;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class CompanyService : ICompanyService
{
    private readonly IRepositoryBase<Company> _companyRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;

    public CompanyService(IRepositoryBase<Company> companyRepository, ICacheService cacheService, IMapper mapper)
    {
        _companyRepository = companyRepository;
        _cacheService = cacheService;
        _mapper = mapper;
    }

    public async Task<List<Company>> Get()
    {
        var cacheData = _cacheService.GetData<List<Company>>("companies");
        if (cacheData == null || !cacheData.Any())
        {
            cacheData = (await _companyRepository
            .GetAsync(navigationProperties: new string[] { "CustomerAdmin" })).ToList();
            var expiryTime = DateTimeOffset.Now.AddSeconds(30);
            _cacheService.SetData("companies", cacheData, expiryTime);
        }
        return cacheData;
    }

    public async Task<Company> GetById(int id)
    {
        var cacheData = _cacheService.GetData<Company>($"company-{id}");
        if (cacheData == null)
        {
            cacheData = await _companyRepository.FoundOrThrow(x => x.Id.Equals(id), new KeyNotFoundException("Company is not exist"));
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
        var cacheList = (await _companyRepository
            .GetAsync(navigationProperties: new string[] { "CustomerAdmin" })).ToList();
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
        var cacheList = (await _companyRepository
            .GetAsync(navigationProperties: new string[] { "CustomerAdmin" })).ToList();
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
        var cacheList = (await _companyRepository
            .GetAsync(navigationProperties: new string[] { "CustomerAdmin" })).ToList();
        _cacheService.SetData("companies", cacheList, expiryTime);
        #endregion
    }   
}
