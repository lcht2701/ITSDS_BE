using API.DTOs.Requests.Teams;
using API.Services.Interfaces;
using Domain.Constants;
using Domain.Customs;
using Domain.Models.Tickets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Helpers;
using System.Net;

namespace API.Controllers;

[Route("/v1/itsds/team")]
public class TeamController : BaseController
{
    private readonly ITeamService _teamService;

    public TeamController(ITeamService teamService)
    {
        _teamService = teamService;
    }

    [Authorize]
    [HttpGet("all")]

    public async Task<IActionResult> GetAllTeam()
    {
        try
        {
            return Ok(await _teamService.Get());
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.ADMIN},{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpGet]
    public async Task<IActionResult> GetTeams(
    [FromQuery] string? filter,
    [FromQuery] string? sort,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 5)
    {
        try
        {
            var teams = await _teamService.Get();
            var pagedResponse = teams.AsQueryable().GetPagedData(page, pageSize, filter, sort);
            return Ok(pagedResponse);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [Authorize(Roles = $"{Roles.ADMIN},{Roles.MANAGER}")]
    [HttpGet("my-teams")]
    public async Task<IActionResult> GetTeamsByManager()
    {
        try
        {
            var result = await _teamService.GetByManager(CurrentUserID);
            return result.Count == 0 ? Ok(result) : Ok("You are currently not managing any team");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.ADMIN},{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpGet("{teamId}")]
    public async Task<IActionResult> GetTeamById(int teamId)
    {
        try
        {
            var result = await _teamService.GetById(teamId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.ADMIN},{Roles.MANAGER}")]
    [HttpPost]
    public async Task<IActionResult> CreateTeam([FromBody] CreateTeamRequest model)
    {
        try
        {
            var result = await _teamService.Create(model);
            return Ok(new { Message = "Team Created Successfully", Data = result });

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.ADMIN},{Roles.MANAGER}")]
    [HttpPut("{teamId}")]
    public async Task<IActionResult> UpdateTeam(int teamId, [FromBody] UpdateTeamRequest model)
    {
        try
        {
            var result = await _teamService.Update(teamId, model);
            return Ok(new { Message = "Team Updated Successfully", Data = result });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.ADMIN},{Roles.MANAGER}")]
    [HttpDelete("{teamId}")]
    public async Task<IActionResult> DeleteTeam(int teamId)
    {
        try
        {
            await _teamService.Remove(teamId);
            return Ok("Team Removed Successfully");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

}
