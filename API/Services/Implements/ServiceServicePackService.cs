using API.Services.Interfaces;
using Domain.Models.Contracts;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class ServiceServicePackService : IServiceServicePackService
{
    private readonly IRepositoryBase<ServiceServicePack> _sspRepository;
    private readonly IRepositoryBase<Service> _serviceRepository;

    public ServiceServicePackService(IRepositoryBase<ServiceServicePack> sspRepository, IRepositoryBase<Service> serviceRepository)
    {
        _sspRepository = sspRepository;
        _serviceRepository = serviceRepository;
    }

    public async Task<List<Service>> GetServices(int packId)
    {
        var serviceIds = (await _sspRepository.WhereAsync(x => x.ServicePackId == packId)).Select(x => x.ServiceId);
        var serviceList = (await _serviceRepository.WhereAsync(x => serviceIds.Contains(x.Id))).ToList();
        return serviceList;
    }

    public async Task AddService(int packId, List<int> serviceIds)
    {
        var newEntities = serviceIds.Select(serviceId => new ServiceServicePack
        {
            ServiceId = serviceId,
            ServicePackId = packId,
        }).ToList();

        await _sspRepository.CreateAsync(newEntities);
    }


    public async Task RemoveService(int id)
    {
        var entity = await _sspRepository.FirstOrDefaultAsync(x => x.Id == id) ?? throw new KeyNotFoundException("Service is not exist in the Service Pack");
        await _sspRepository.DeleteAsync(entity);
    }
}
