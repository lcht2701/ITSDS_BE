using API.Mappings;
using Domain.Constants.Enums;
using Domain.Models;

namespace API.DTOs.Responses.Dashboards.Admins;

public class UserRolesCountDashboard
{
    public List<UserCountDashboard> data { get; set; }
    public int Total;
}

public class UserCountDashboard
{
    public string Row { get; set;}
    public int Amount { get; set;}
}

public class UserActiveDashboardData
{
    public int ActiveUserCount { get; set; }
    public int InactiveUserCount { get; set; }
}

public class UserCreatedDashboardData : IMapFrom<User>
{
    public string? Username { get; set; }

    public string? Role { get; set; }

    public DateTime? CreatedAt { get; set; }
}

public class UserUpdatedDashboardData : IMapFrom<User>
{
    public string? Username { get; set; }

    public string? Role { get; set; }

    public DateTime? ModifiedAt { get; set; }
}
