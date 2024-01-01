using API.DTOs.Requests.Modes;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Models.Tickets;
using Persistence.Helpers.Caching;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class ModeService : IModeService
{
    private readonly IRepositoryBase<Mode> _moderepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;

    public ModeService(IRepositoryBase<Mode> moderepository, ICacheService cacheService, IMapper mapper)
    {
        _moderepository = moderepository;
        _cacheService = cacheService;
        _mapper = mapper;
    }

    public async Task<List<Mode>> Get()
    {
        var cacheData = _cacheService.GetData<List<Mode>>("modes");
        if (cacheData == null || !cacheData.Any())
        {
            cacheData = await _moderepository.ToListAsync();
            var expiryTime = DateTimeOffset.Now.AddSeconds(30);
            _cacheService.SetData("modes", cacheData, expiryTime);
        }
        return cacheData;
    }

    public async Task<Mode> GetById(int id)
    {
        var cacheData = _cacheService.GetData<Mode>($"mode-{id}");
        if (cacheData == null)
        {
            cacheData = await _moderepository.FoundOrThrow(x => x.Id.Equals(id), new KeyNotFoundException("Mode is not exist"));
            var expiryTime = DateTimeOffset.Now.AddSeconds(30);
            _cacheService.SetData($"mode-{cacheData.Id}", cacheData, expiryTime);
        }
        return cacheData;
    }

    public async Task<Mode> Create(CreateModeRequest model)
    {
        var entity = _mapper.Map(model, new Mode());
        var result = await _moderepository.CreateAsync(entity);
        #region Cache
        var expiryTime = DateTimeOffset.Now.AddSeconds(30);
        _cacheService.SetData($"mode-{result.Id}", result, expiryTime);
        var cacheList = await _moderepository.ToListAsync();
        _cacheService.SetData("modes", cacheList, expiryTime);
        #endregion
        return entity;
    }

    public async Task<Mode> Update(int id, UpdateModeRequest model)
    {
        var target = await _moderepository.FoundOrThrow(c => c.Id.Equals(id), new KeyNotFoundException("Mode is not exist"));
        Mode entity = _mapper.Map(model, target);
        var result = await _moderepository.UpdateAsync(entity);
        #region Cache
        var expiryTime = DateTimeOffset.Now.AddSeconds(30);
        _cacheService.SetData($"mode-{result.Id}", result, expiryTime);
        var cacheList = await _moderepository.ToListAsync();
        _cacheService.SetData("modes", cacheList, expiryTime);
        #endregion
        return entity;
    }

    public async Task Remove(int id)
    {
        var target = await _moderepository.FoundOrThrow(c => c.Id.Equals(id), new KeyNotFoundException("Mode is not exist"));
        await _moderepository.SoftDeleteAsync(target);
        #region Cache
        var expiryTime = DateTimeOffset.Now.AddSeconds(30);
        _cacheService.RemoveData($"mode-{target.Id}");
        var cacheList = await _moderepository.ToListAsync();
        _cacheService.SetData("modes", cacheList, expiryTime);
        #endregion
    }
}
