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

    public async Task<List<User>> GetMembersNotInTeam(int teamId)
    {
        var teamMembers = await _teamMemberRepository.WhereAsync(u => u.TeamId.Equals(teamId)) ??
                          throw new KeyNotFoundException();
        var userIds = teamMembers.Select(tm => tm.MemberId).ToList();
        var users = await _userRepository.WhereAsync(user => !userIds.Contains(user.Id) &&
                                                             !(user.Role == Role.Customer || user.Role == Role.Admin));
        return (List<User>)users;
    }

    public async Task<List<User>> GetByTeam(int teamId)
    {
        var teamMembers = await _teamMemberRepository.WhereAsync(u => u.TeamId.Equals(teamId)) ??
                          throw new KeyNotFoundException();
        var userIds = teamMembers.Select(tm => tm.MemberId).ToList();
        var users = await _userRepository.WhereAsync(u => userIds.Contains(u.Id));
        return (List<User>)users;
    }

    public async Task Assign(AssignMemberToTeamRequest model)
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

    public async Task Update(int memberId, UpdateTeamMemberRequest model)
    {
        var member = await _teamMemberRepository.FirstOrDefaultAsync(x => x.MemberId.Equals(memberId)) ??
                     throw new KeyNotFoundException("Team member is not exist");
        var entity = _mapper.Map(model, member);
        await _teamMemberRepository.UpdateAsync(entity);
    }

    public async Task Transfer(int memberId, int newTeamId)
    {
        var user = await _teamMemberRepository.FirstOrDefaultAsync(x => x.MemberId.Equals(memberId)) ??
                   throw new KeyNotFoundException("User is currently not a member of any team");

        if (user.TeamId == newTeamId)
        {
            throw new BadRequestException("Cannot transfer in the same team");
        }

        user.TeamId = newTeamId;
        await _teamMemberRepository.UpdateAsync(user);
    }

    public async Task Remove(int memberId)
    {
        var user = await _teamMemberRepository.FirstOrDefaultAsync(x => x.MemberId.Equals(memberId)) ??
                   throw new KeyNotFoundException("User is currently not a member of any team");
        await _teamMemberRepository.DeleteAsync(user);
    }
}