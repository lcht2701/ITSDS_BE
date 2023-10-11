using API.DTOs.Requests.TeamMembers;
using Domain.Constants;
using Domain.Constants.Enums;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Tickets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Repositories.Interfaces;

namespace API.Controllers;

[Route("/v1/itsds/team/member/")]
public class TeamMemberController : BaseController
{
    private readonly IRepositoryBase<Team> _teamRepository;
    private readonly IRepositoryBase<TeamMember> _teamMemberRepository;
    private readonly IRepositoryBase<User> _userRepository;

    public TeamMemberController(IRepositoryBase<Team> teamRepository, IRepositoryBase<TeamMember> teamMemberRepository, IRepositoryBase<User> userRepository)
    {
        _teamRepository = teamRepository;
        _teamMemberRepository = teamMemberRepository;
        _userRepository = userRepository;
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpGet("{teamId}")]
    public async Task<IActionResult> GetTeamMembers(int teamId)
    {
        var result = await _teamMemberRepository.WhereAsync(u => u.TeamId.Equals(teamId));
        return Ok(result);
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpPost("assign")]
    public async Task<IActionResult> AssignMemberToTeam([FromBody] AssignMemberToTeamRequest model)
    {
        var user = await _userRepository.FoundOrThrow(x => x.Id.Equals(model.MemberId), new BadRequestException("User not found"));
        if (user.Role != Role.Accountant || user.Role != Role.Technician)
        {
            throw new BadRequestException("This user is not allowed to assign to a team");
        }
        var team = await _teamRepository.FoundOrThrow(x => x.Id.Equals(model.TeamId), new BadRequestException("Team not found"));
        var isInTeam = await _teamMemberRepository.FirstOrDefaultAsync(x => x.MemberId.Equals(user.Id));
        if (isInTeam != null)
        {
            throw new BadRequestException("User is already in a team");
        }

        var entity = Mapper.Map(model, new TeamMember());
        await _teamMemberRepository.CreateAsync(entity);
        return Ok("Successfully");
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateTeamMember([FromBody] UpdateTeamMemberRequest model, int teamId, int memberId)
    {
        var user = await _teamMemberRepository.FoundOrThrow(x => x.TeamId.Equals(teamId) && x.MemberId.Equals(memberId), new NotFoundException("Member is not in this team."));
        TeamMember entity = Mapper.Map(model, user);
        await _teamMemberRepository.CreateAsync(entity);
        return Ok("Successfully");
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpPatch("transfer")]
    public async Task<IActionResult> TransferTeamMember(int memberId, int newTeamId)
    {
        var user = await _teamMemberRepository.FoundOrThrow(x => x.MemberId.Equals(memberId), new BadRequestException("User is currently not a member of any team"));

        if (user.TeamId == newTeamId)
        {
            throw new BadRequestException("Cannot transfer in the same team");
        }

        user.TeamId = newTeamId;
        await _teamMemberRepository.UpdateAsync(user);
        return Ok("Successfully");
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpDelete("remove")]
    public async Task<IActionResult> RemoveTeamMember(int memberId)
    {
        var user = await _teamMemberRepository.FoundOrThrow(x => x.MemberId.Equals(memberId), new BadRequestException("User is currently not a member of any team"));

        await _teamMemberRepository.DeleteAsync(user);
        return Ok("Successfully");
    }





}
