using API.Mappings;
using Domain.Models.Tickets;

namespace API.DTOs.Responses.Dashboards.Admins;

public class TeamActiveDashboardData
{
    public int ActiveTeamCount { get; set; }
    public int InactiveTeamCount { get; set; }
}

public class TeamMemberDashboardData
{
    public List<TeamMemberCountData> data { get; set; }
    public int TotalCount { get; set; }
}

public class TeamMemberCountData
{
    public string? Name { get; set; }

    public int? NumberOfMembers { get; set; }
}

public class TeamCreatedDashboardData :IMapFrom<Team>
{
    public string? Name { get; set; }

    public string? Location { get; set; }

    public DateTime? CreatedAt { get; set; }

}

public class TeamUpdatedDashboardData : IMapFrom<Team>
{
    public string? Name { get; set; }

    public string? Location { get; set; }

    public DateTime? ModifiedAt { get; set; }

}
