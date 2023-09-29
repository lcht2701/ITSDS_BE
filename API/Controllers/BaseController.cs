using AutoMapper;
using Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
public class BaseController : ControllerBase
{
    private IMapper _mapper;

    protected IMapper Mapper => _mapper ??= HttpContext.RequestServices.GetService<IMapper>();

    protected int CurrentUserID => GetUserID();

    protected int GetUserID()
    {
        var userIDClaim = HttpContext.User.Claims.FirstOrDefault(a => a.Type == "id");
        if (userIDClaim is null)
        {
            return -1; // or any default value you prefer for the case when the claim is not found
        }

        if (int.TryParse(userIDClaim.Value, out int userID))
        {
            return userID;
        }

        // Handle the case where the claim value is not a valid integer.
        // You might want to throw an exception or return a default value.

        return -1; // Default value if parsing fails
    }

    public bool IsAdmin => IsInRole(Roles.ADMIN);
    public bool IsManager => IsInRole(Roles.MANAGER);
    public bool IsCustomer => IsInRole(Roles.CUSTOMER);
    public bool IsTechnician => IsInRole(Roles.TECHNICIAN);
    public bool IsAccountant => IsInRole(Roles.ACCOUNTANT);
    private bool IsInRole(string role) {
        return User.IsInRole(role);
    }
}
