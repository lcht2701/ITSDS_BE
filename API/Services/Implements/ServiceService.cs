using API.DTOs.Requests.Services;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Models.Contracts;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class ServiceService : IServiceService
{
    private readonly IRepositoryBase<Service> _serviceRepository;
    private readonly IMapper _mapper;

    public ServiceService(IRepositoryBase<Service> serviceRepository, IMapper mapper)
    {
        _serviceRepository = serviceRepository;
        _mapper = mapper;
    }

    public async Task<List<Service>> Get()
    {
        var result = await _serviceRepository.ToListAsync();
        return result;
    }

    public async Task<Service> GetById(int id)
    {
        var result = await _serviceRepository.FirstOrDefaultAsync(u => u.Id.Equals(id), new string[] { "ServicePack" }) ?? throw new KeyNotFoundException("Service is not exist");
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
