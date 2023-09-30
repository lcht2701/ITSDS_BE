using API.DTOs.Requests.TeamMembers;
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

    [Authorize]
    [HttpGet("{teamId}")]
    public async Task<IActionResult> GetTeamMembers(int teamId)
    {
        var result = await _teamMemberRepository.WhereAsync(u => u.TeamId.Equals(teamId));
        return Ok(result);
    }

    [Authorize]
    [HttpPost("assign-member")]
    public async Task<IActionResult> AssignMemberToTeam([FromBody] AssignMemberToTeamRequest model)
    {
        var user = await _userRepository.FoundOrThrow(x => x.Id.Equals(model.MemberId), new BadRequestException("User not found"));
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
}
