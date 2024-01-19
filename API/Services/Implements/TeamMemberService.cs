using API.DTOs.Requests.TeamMembers;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Constants.Enums;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Tickets;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class TeamMemberService : ITeamMemberService
{
    private readonly IRepositoryBase<TeamMember> _teamMemberRepository;
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IMapper _mapper;

    public TeamMemberService(IRepositoryBase<TeamMember> teamMemberRepository, IRepositoryBase<User> userRepository,
        IMapper mapper)
    {
        _teamMemberRepository = teamMemberRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<List<TeamMember>> Get()
    {
        var result = await _teamMemberRepository.GetAsync(navigationProperties: new string[] { "Member", "Team" });
        return result.ToList();
    }

    public async Task<TeamMember> GetById(int id)
    {
        var result = await _teamMemberRepository.FirstOrDefaultAsync(x => x.Id == id, new string[] { "Member", "Team" })
            ?? throw new KeyNotFoundException("Team Member is not exist");
        return result;
    }

    public async Task<List<User>> GetMembersNotInTeam(int teamId)
    {
        var teamMembers = await _teamMemberRepository.WhereAsync(u => u.TeamId.Equals(teamId));
        var userIds = teamMembers.Select(tm => tm.MemberId).ToList();
        var users = await _userRepository.WhereAsync(user => !userIds.Contains(user.Id) &&
                                                             !(user.Role == Role.Customer || user.Role == Role.Admin));
        return (List<User>)users;
    }

    public async Task<List<TeamMember>> GetMembersInTeam(int teamId)
    {
        var teamMembers = await _teamMemberRepository.WhereAsync(u => u.TeamId.Equals(teamId), new string[] { "Member", "Team" });
        return teamMembers.ToList();
    }

    public async Task Add(AddMemberToTeamRequest model)
    {
        var user = await _userRepository.FirstOrDefaultAsync(x => x.Id.Equals(model.MemberId)) ??
                   throw new KeyNotFoundException("User is not exist");
        if (user.Role == Role.Customer || user.Role == Role.Admin)
        {
            throw new BadRequestException("This user is not allowed to assign to a team");
        }

        var isInTeam = await _teamMemberRepository.FirstOrDefaultAsync(x =>
            x.MemberId.Equals(model.MemberId) && x.TeamId.Equals(model.TeamId));
        if (isInTeam != null)
        {
            throw new BadRequestException("User is already in team");
        }

        var entity = _mapper.Map(model, new TeamMember());
        await _teamMemberRepository.CreateAsync(entity);
    }

    public async Task Update(int id, UpdateTeamMemberRequest model)
    {
        var member = await _teamMemberRepository.FirstOrDefaultAsync(x => x.Id.Equals(id)) ??
                     throw new KeyNotFoundException("Team member is not exist");
        var entity = _mapper.Map(model, member);
        await _teamMemberRepository.UpdateAsync(entity);
    }

    public async Task Remove(int id)
    {
        var user = await _teamMemberRepository.FirstOrDefaultAsync(x => x.Id.Equals(id)) ??
                   throw new KeyNotFoundException("User is currently not a member of any team");
        await _teamMemberRepository.DeleteAsync(user);
    }
}