using API.DTOs.Requests.TeamMembers;
using API.Services.Interfaces;
using Domain.Constants;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Helpers;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

[Route("/v1/itsds/team/member/")]
public class TeamMemberController : BaseController
{
    private readonly ITeamMemberService _teamMemberService;

    public TeamMemberController(ITeamMemberService teamMemberService)
    {
        _teamMemberService = teamMemberService;
    }

    [Authorize(Roles = $"{Roles.MANAGER}")]
    [HttpGet]
    public async Task<IActionResult> Get(
    [FromQuery] string? filter,
    [FromQuery] string? sort,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 5)
    {
        try
        {
            var members = await _teamMemberService.Get();
            var pagedResponse = members.AsQueryable().GetPagedData(page, pageSize, filter, sort);
            int totalPage = (int)Math.Ceiling((double)members.Count / pageSize);
            return Ok(new { TotalPage = totalPage, Data = pagedResponse });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpGet("{teamId}")]
    [SwaggerResponse(200, "Get Member In Team", typeof(List<Domain.Models.User>))]
    public async Task<IActionResult> GetMembersInTeam(int teamId)
    {
        try
        {
            var members = await _teamMemberService.GetMembersInTeam(teamId);
            return Ok(members);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Team is not exist");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpGet("select-list")]
    [SwaggerResponse(200, "Get Member Not In Team", typeof(List<Domain.Models.User>))]
    public async Task<IActionResult> GetMembersNotInTeam(int teamId)
    {
        try
        {
            var members = await _teamMemberService.GetMembersNotInTeam(teamId);
            return Ok(members);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Team is not exist");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpGet("get/{id}")]
    [SwaggerResponse(200, "Get Member By Id", typeof(Domain.Models.Tickets.TeamMember))]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var members = await _teamMemberService.GetById(id);
            return Ok(members);
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


    [Authorize(Roles = Roles.MANAGER)]
    [HttpPost("assign")]
    public async Task<IActionResult> AssignMemberToTeam([FromBody] AddMemberToTeamRequest model)
    {
        try
        {
            await _teamMemberService.Add(model);
            return Ok("Assigned Successfully");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (BadRequestException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateTeamMember(int id, [FromBody] UpdateTeamMemberRequest model)
    {
        try
        {
            await _teamMemberService.Update(id, model);
            return Ok("Successfully");
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

    [Authorize(Roles = Roles.MANAGER)]
    [HttpDelete("remove")]
    public async Task<IActionResult> RemoveTeamMember(int id)
    {
        try
        {
            await _teamMemberService.Remove(id);
            return Ok("Successfully");
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