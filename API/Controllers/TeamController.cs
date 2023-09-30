using API.DTOs.Requests.Teams;
using API.DTOs.Requests.Users;
using Domain.Constants;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Tickets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Repositories.Interfaces;

namespace API.Controllers;

[Route("/v1/itsds/team")]
public class TeamController : BaseController
{
    private readonly IRepositoryBase<Team> _teamRepository;
    private readonly IRepositoryBase<TeamMember> _teamMemberRepository;
    private readonly IRepositoryBase<User> _userRepository;

    public TeamController(IRepositoryBase<Team> teamRepository, IRepositoryBase<TeamMember> teamMemberRepository, IRepositoryBase<User> userRepository)
    {
        _teamRepository = teamRepository;
        _teamMemberRepository = teamMemberRepository;
        _userRepository = userRepository;
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpGet]
    public async Task<IActionResult> GetTeams()
    {
        var result = await _teamRepository.WhereAsync(x => x.ManagerId.Equals(CurrentUserID));
        return Ok(result);
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpGet("{teamId}")]
    public async Task<IActionResult> GetTeamById(int teamId)
    {
        var result = await _teamRepository.FoundOrThrow(x => x.Id.Equals(teamId), new NotFoundException("Team not found"));
        return Ok(result);
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpPost]
    public async Task<IActionResult> CreateTeam([FromBody] CreateTeamRequest model)
    {
        var entity = Mapper.Map(model, new Team());
        entity.IsActive = true;
        entity.ManagerId = CurrentUserID;
        await _teamRepository.CreateAsync(entity);
        return Ok();
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpPut("{teamId}")]
    public async Task<IActionResult> UpdateTeam(int teamId, [FromBody] UpdateTeamRequest req)
    {
        var target = await _teamRepository.FoundOrThrow(c => c.Id.Equals(teamId), new NotFoundException("Team not found"));
        Team entity = Mapper.Map(req, target);
        await _teamRepository.UpdateAsync(entity);
        return Accepted("Updated Successfully");
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpDelete("{teamId}")]
    public async Task<IActionResult> DeleteTeam(int teamId)
    {
        var target = await _teamRepository.FoundOrThrow(c => c.Id.Equals(teamId), new NotFoundException("Team not found"));
        //Soft Delete
        await _teamRepository.DeleteAsync(target);
        return Ok("Deleted Successfully");
    }

}
