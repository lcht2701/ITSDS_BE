﻿using API.DTOs.Requests.Teams;
using Domain.Constants;
using Domain.Exceptions;
using Domain.Models.Tickets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    [Authorize(Roles = $"{Roles.ADMIN},{Roles.MANAGER}")]
    [HttpGet]
    public async Task<IActionResult> GetTeams(int page = 1, int pageSize = 3)
    {
        var teams = await _teamRepository.ToListAsync();
        var totalCount = teams.Count;
        var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
        var teamsPerPage = teams
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        return Ok(teamsPerPage);
    }

    [Authorize(Roles = $"{Roles.ADMIN},{Roles.MANAGER}")]
    [HttpGet("my-teams")]
    public async Task<IActionResult> GetTeamsByManager()
    {
        var result = await _teamRepository.WhereAsync(x => x.ManagerId.Equals(CurrentUserID));
        return result.Count == 0 ? Ok(result) : Ok("You are currently not managing any team");
    }

    [Authorize(Roles = $"{Roles.ADMIN},{Roles.MANAGER}")]
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
        return Accepted("Updated Successfully");
    }

    [Authorize(Roles = $"{Roles.ADMIN},{Roles.MANAGER}")]
    [HttpDelete("{teamId}")]
    public async Task<IActionResult> DeleteTeam(int teamId)
    {
        var target = await _teamRepository.FoundOrThrow(c => c.Id.Equals(teamId), new BadRequestException("Team not found"));
        //Soft Delete
        await _teamRepository.DeleteAsync(target);
        return Ok("Deleted Successfully");
    }

}
