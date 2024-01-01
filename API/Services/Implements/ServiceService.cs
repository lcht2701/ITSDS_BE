using API.DTOs.Requests.Services;
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
        var cacheData = _cacheService.GetData<List<Service>>("services");
        if (cacheData == null || !cacheData.Any())
        {
            cacheData = await _serviceRepository.ToListAsync();
            var expiryTime = DateTimeOffset.Now.AddSeconds(30);
            _cacheService.SetData("services", cacheData, expiryTime);
        }
        return cacheData;
    }

    public async Task<List<Service>> GetByCategory(int categoryId)
    {
        var result = (await _serviceRepository.WhereAsync(x => x.CategoryId.Equals(categoryId))).ToList() ?? throw new KeyNotFoundException("Category is not exist");
        return result;
    }

    public async Task<Service> GetById(int id)
    {
        var cacheData = _cacheService.GetData<Service>($"service-{id}");
        if (cacheData == null)
        {
            cacheData = await _serviceRepository.FirstOrDefaultAsync(u => u.Id.Equals(id)) ?? throw new KeyNotFoundException("Service is not exist");
            var expiryTime = DateTimeOffset.Now.AddSeconds(30);
            _cacheService.SetData($"service-{cacheData.Id}", cacheData, expiryTime);
        }
        return cacheData;
    }

    public async Task<Service> Create(CreateServiceRequest model)
    {
        var entity = _mapper.Map(model, new Service());
        var result = await _serviceRepository.CreateAsync(entity);
        #region Cache
        var expiryTime = DateTimeOffset.Now.AddSeconds(30);
        _cacheService.SetData($"service-{result.Id}", result, expiryTime);
        var cacheList = await _serviceRepository.ToListAsync();
        _cacheService.SetData("services", cacheList, expiryTime);
        #endregion
        return result;
    }

    public async Task<Service> Update(int id, UpdateServiceRequest model)
    {
        var target = await _serviceRepository.FoundOrThrow(c => c.Id.Equals(id), new KeyNotFoundException("Service is not exist"));
        Service entity = _mapper.Map(model, target);
        var result = await _serviceRepository.UpdateAsync(entity);
        #region Cache
        var expiryTime = DateTimeOffset.Now.AddSeconds(30);
        _cacheService.SetData($"service-{result.Id}", result, expiryTime);
        var cacheList = await _serviceRepository.ToListAsync();
        _cacheService.SetData("services", cacheList, expiryTime);
        #endregion
        return result;
    }

    public async Task Remove(int id)
    {
        var target = await _serviceRepository.FoundOrThrow(c => c.Id.Equals(id), new KeyNotFoundException("Service is not exist"));
        await _serviceRepository.SoftDeleteAsync(target);
        #region Cache
        var expiryTime = DateTimeOffset.Now.AddSeconds(30);
        _cacheService.RemoveData($"service-{target.Id}");
        var cacheList = await _serviceRepository.ToListAsync();
        _cacheService.SetData("services", cacheList, expiryTime);
        #endregion
    }

}
