using API.DTOs.Requests.Teams;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Models.Tickets;
using Persistence.Helpers.Caching;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class TeamService : ITeamService
{
    private readonly IRepositoryBase<Team> _teamRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;

    public TeamService(IRepositoryBase<Team> teamRepository, ICacheService cacheService, IMapper mapper)
    {
        _teamRepository = teamRepository;
        _cacheService = cacheService;
        _mapper = mapper;
    }

    public async Task<List<Team>> Get()
    {
        var cacheData = _cacheService.GetData<List<Team>>("teams");
        if (cacheData == null || !cacheData.Any())
        {
            cacheData = (await _teamRepository
                .GetAsync(navigationProperties: new string[] { "Manager" }))
                .ToList();
            var expiryTime = DateTimeOffset.Now.AddSeconds(30);
            _cacheService.SetData("teams", cacheData, expiryTime);
        }
        return cacheData; 
    }

    public async Task<Team> GetById(int id)
    {
        var cacheData = _cacheService.GetData<Team>($"team-{id}");
        if (cacheData == null)
        {
            cacheData = await _teamRepository.FirstOrDefaultAsync(x => x.Id.Equals(id), new string[] { "Manager" }) ?? throw new KeyNotFoundException("Team is not exist");
            var expiryTime = DateTimeOffset.Now.AddSeconds(30);
            _cacheService.SetData($"team-{cacheData.Id}", cacheData, expiryTime);
        }
        return cacheData;
    }

    public async Task<List<Team>> GetByManager(int managerId)
    {
        var result = (await _teamRepository.WhereAsync(x => x.ManagerId.Equals(managerId), new string[] { "Manager" })).ToList();
        return result;
    }

    public async Task<Team> Create(CreateTeamRequest model)
    {
        var entity = _mapper.Map(model, new Team());
        entity.IsActive = true;
        var result = await _teamRepository.CreateAsync(entity);
        #region Cache
        var expiryTime = DateTimeOffset.Now.AddSeconds(30);
        _cacheService.SetData($"team-{result.Id}", result, expiryTime);
        var cacheList = (await _teamRepository
                .GetAsync(navigationProperties: new string[] { "Manager" }))
                .ToList();
        _cacheService.SetData("teams", cacheList, expiryTime);
        #endregion
        return result;
    }

    public async Task<Team> Update(int id, UpdateTeamRequest model)
    {
        var target = await _teamRepository.FoundOrThrow(c => c.Id.Equals(id), new KeyNotFoundException("Team is not exist"));
        Team entity = _mapper.Map(model, target);
        var result = await _teamRepository.UpdateAsync(entity);
        #region Cache
        var expiryTime = DateTimeOffset.Now.AddSeconds(30);
        _cacheService.SetData($"team-{result.Id}", result, expiryTime);
        var cacheList = (await _teamRepository
                .GetAsync(navigationProperties: new string[] { "Manager" }))
                .ToList();
        _cacheService.SetData("teams", cacheList, expiryTime);
        #endregion
        return target;
    }
    public async Task Remove(int id)
    {
        var target = await _teamRepository.FoundOrThrow(c => c.Id.Equals(id), new KeyNotFoundException("Team is not exist"));
        await _teamRepository.SoftDeleteAsync(target);
        #region Cache
        var expiryTime = DateTimeOffset.Now.AddSeconds(30);
        _cacheService.RemoveData($"team-{target.Id}");
        var cacheList = (await _teamRepository
                .GetAsync(navigationProperties: new string[] { "Manager" }))
                .ToList();
        _cacheService.SetData("teams", cacheList, expiryTime);
        #endregion
    }
}
