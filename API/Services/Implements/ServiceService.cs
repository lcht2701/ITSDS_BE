﻿using API.DTOs.Requests.Services;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Models.Contracts;
using Persistence.Helpers.Caching;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class ServiceService : IServiceService
{
    private readonly IRepositoryBase<Service> _serviceRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;

    public ServiceService(IRepositoryBase<Service> serviceRepository, ICacheService cacheService, IMapper mapper)
    {
        _serviceRepository = serviceRepository;
        _cacheService = cacheService;
        _mapper = mapper;
    }

    public async Task<List<Service>> Get()
    {
        return await _cacheService.GetAsync(
            "services",
            async () => await _serviceRepository.ToListAsync());
    }

    public async Task<List<Service>> GetByCategory(int categoryId)
    {
        var result = (await _serviceRepository.WhereAsync(x => x.CategoryId.Equals(categoryId))).ToList() ?? throw new KeyNotFoundException("Category is not exist");
        return result;
    }

    public async Task<Service> GetById(int id)
    {
        var result = await _serviceRepository.FirstOrDefaultAsync(u => u.Id.Equals(id)) ?? throw new KeyNotFoundException("Service is not exist");
        return result;
    }

    public async Task<Service> Create(CreateServiceRequest model)
    {
        var entity = _mapper.Map(model, new Service());
        await _serviceRepository.CreateAsync(entity);
        return entity;
    }

    public async Task<Service> Update(int id, UpdateServiceRequest model)
    {
        var target = await _serviceRepository.FoundOrThrow(c => c.Id.Equals(id), new KeyNotFoundException("Service is not exist"));
        Service entity = _mapper.Map(model, target);
        await _serviceRepository.UpdateAsync(entity);
        return entity;
    }

    public async Task Remove(int id)
    {
        var target = await _serviceRepository.FoundOrThrow(c => c.Id.Equals(id), new KeyNotFoundException("Service is not exist"));
        await _serviceRepository.SoftDeleteAsync(target);
    }

}
