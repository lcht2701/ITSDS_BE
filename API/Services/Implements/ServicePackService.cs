using API.DTOs.Requests.ServicePacks;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Models.Contracts;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class ServicePackService : IServicePackService
{
    private readonly IRepositoryBase<ServicePack> _servicePackRepository;
    private readonly IMapper _mapper;

    public ServicePackService(IRepositoryBase<ServicePack> servicePackRepository, IMapper mapper)
    {
        _servicePackRepository = servicePackRepository;
        _mapper = mapper;
    }

    public async Task<List<ServicePack>> Get()
    {
        var result = await _servicePackRepository.ToListAsync();
        return result;
    }

    public async Task<ServicePack> GetById(int id)
    {
        var result = await _servicePackRepository.FirstOrDefaultAsync(u => u.Id.Equals(id)) ?? throw new KeyNotFoundException("Service Pack is not exist");
        return result;
    }

    public async Task<ServicePack> Create(CreateServicePackRequest model)
    {
        var entity = _mapper.Map(model, new ServicePack());
        await _servicePackRepository.CreateAsync(entity);
        return entity;
    }

    public async Task<ServicePack> Update(int id, UpdateServicePackRequest model)
    {
        var target = await _servicePackRepository.FoundOrThrow(c => c.Id.Equals(id), new KeyNotFoundException("Service Pack is not exist"));
        ServicePack entity = _mapper.Map(model, target);
        await _servicePackRepository.UpdateAsync(entity);
        return entity;
    }

    public async Task Remove(int id)
    {
        var target = await _servicePackRepository.FoundOrThrow(c => c.Id.Equals(id), new KeyNotFoundException("Service Pack is not exist"));
        await _servicePackRepository.SoftDeleteAsync(target);
    }
}
