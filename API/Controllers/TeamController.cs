using API.DTOs.Requests.Teams;
using Domain.Constants;
using Domain.Exceptions;
using Domain.Models.Tickets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Helpers;
using Persistence.Repositories.Interfaces;

namespace API.Controllers;

[Route("/v1/itsds/team")]
public class TeamController : BaseController
{
    private readonly IRepositoryBase<Team> _teamRepository;

    public TeamController(IRepositoryBase<Team> teamRepository)
    {
        _teamRepository = teamRepository;
    }

    [Authorize]
    [HttpGet("all")]

    public async Task<IActionResult> GetAllTeam()
    {
        var result = await _teamRepository.ToListAsync();
        return Ok(result);
    }

    [Authorize(Roles = $"{Roles.ADMIN},{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpGet]
    public async Task<IActionResult> GetTeams(
    [FromQuery] string? filter,
    [FromQuery] string? sort,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 5)
    {
        var teams = await _teamRepository.ToListAsync();
        var pagedResponse = teams.AsQueryable().GetPagedData(page, pageSize, filter, sort);
        return Ok(pagedResponse);
    }


    [Authorize(Roles = $"{Roles.ADMIN},{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpGet("my-teams")]
    public async Task<IActionResult> GetTeamsByManager()
    {
        var result = await _teamRepository.WhereAsync(x => x.ManagerId.Equals(CurrentUserID));
        return result.Count == 0 ? Ok(result) : Ok("You are currently not managing any team");
    }

    [Authorize(Roles = $"{Roles.ADMIN},{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpGet("{teamId}")]
    public async Task<IActionResult> GetTeamById(int teamId)
    {
        var result = await _teamRepository.FoundOrThrow(x => x.Id.Equals(teamId), new BadRequestException("Team not found"));
        return Ok(result);
    }

    [Authorize(Roles = $"{Roles.ADMIN},{Roles.MANAGER}")]
    [HttpPost]
    public async Task<IActionResult> CreateTeam([FromBody] CreateTeamRequest model)
    {
        var entity = Mapper.Map(model, new Team());
        entity.IsActive = true;
        await _teamRepository.CreateAsync(entity);
        return Ok();
    }

    [Authorize(Roles = $"{Roles.ADMIN},{Roles.MANAGER}")]
    [HttpPut("{teamId}")]
    public async Task<IActionResult> UpdateTeam(int teamId, [FromBody] UpdateTeamRequest req)
    {
        var target = await _teamRepository.FoundOrThrow(c => c.Id.Equals(teamId), new BadRequestException("Team not found"));
        Team entity = Mapper.Map(req, target);
        await _teamRepository.UpdateAsync(entity);
        return Accepted("Update Successfully");
    }

    [Authorize(Roles = $"{Roles.ADMIN},{Roles.MANAGER}")]
    [HttpDelete("{teamId}")]
    public async Task<IActionResult> DeleteTeam(int teamId)
    {
        var target = await _teamRepository.FoundOrThrow(c => c.Id.Equals(teamId), new BadRequestException("Team not found"));
        await _teamRepository.SoftDeleteAsync(target);
        return Ok("Delete Successfully");
    }

}
