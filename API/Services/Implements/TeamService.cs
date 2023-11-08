using API.DTOs.Requests.Teams;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Models.Tickets;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class TeamService : ITeamService
{
    private readonly IRepositoryBase<Team> _teamRepository;
    private readonly IMapper _mapper;

    public TeamService(IRepositoryBase<Team> teamRepository, IMapper mapper)
    {
        _teamRepository = teamRepository;
        _mapper = mapper;
    }

    public async Task<List<Team>> Get()
    {
        var result = await _teamRepository.ToListAsync();
        return result;
    }

    public async Task<Team> GetById(int id)
    {
        var result = await _teamRepository.FoundOrThrow(x => x.Id.Equals(id), new KeyNotFoundException("Team is not exist"));
        return result;
    }

    public async Task<List<Team>> GetByManager(int managerId)
    {
        var result = (await _teamRepository.WhereAsync(x => x.ManagerId.Equals(managerId))).ToList();
        return result;
    }

    public async Task<Team> Create(CreateTeamRequest model)
    {
        var entity = _mapper.Map(model, new Team());
        entity.IsActive = true;
        await _teamRepository.CreateAsync(entity);
        return entity;
    }

    public async Task<Team> Update(int id, UpdateTeamRequest model)
    {
        var target = await _teamRepository.FoundOrThrow(c => c.Id.Equals(id), new KeyNotFoundException("Team is not exist"));
        Team entity = _mapper.Map(model, target);
        await _teamRepository.UpdateAsync(entity);
        return target;
    }
    public async Task Remove(int id)
    {
        var target = await _teamRepository.FoundOrThrow(c => c.Id.Equals(id), new KeyNotFoundException("Team is not exist"));
        await _teamRepository.SoftDeleteAsync(target);
    }
}
