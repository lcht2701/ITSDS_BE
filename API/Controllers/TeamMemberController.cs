using API.DTOs.Requests.TeamMembers;
using API.Services.Interfaces;
using Domain.Constants;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("/v1/itsds/team/member/")]
public class TeamMemberController : BaseController
{
    private readonly ITeamMemberService _teamMemberService;

    public TeamMemberController(ITeamMemberService teamMemberService)
    {
        _teamMemberService = teamMemberService;
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpGet("{teamId}")]
    public async Task<IActionResult> GetTeamMembersByTeam(int teamId)
    {
        try
        {
            var members = await _teamMemberService.GetByTeam(teamId);
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
    public async Task<IActionResult> GetSelectList(int teamId)
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


    [Authorize(Roles = Roles.MANAGER)]
    [HttpPost("assign")]
    public async Task<IActionResult> AssignMemberToTeam([FromBody] AssignMemberToTeamRequest model)
    {
        try
        {
            await _teamMemberService.Assign(model);
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
    public async Task<IActionResult> UpdateTeamMember(int memberId, [FromBody] UpdateTeamMemberRequest model)
    {
        try
        {
            await _teamMemberService.Update(memberId, model);
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
    [HttpPatch("transfer")]
    public async Task<IActionResult> TransferTeamMember(int memberId, int newTeamId)
    {
        try
        {
            await _teamMemberService.Transfer(memberId, newTeamId);
            return Ok("Successfully");
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
    [HttpDelete("remove")]
    public async Task<IActionResult> RemoveTeamMember(int memberId)
    {
        try
        {
            await _teamMemberService.Remove(memberId);
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