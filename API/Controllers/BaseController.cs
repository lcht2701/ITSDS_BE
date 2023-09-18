using AutoMapper;
using Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
public class BaseController : ControllerBase
{
    private IMapper _mapper;

    protected IMapper Mapper => _mapper ??= HttpContext.RequestServices.GetService<IMapper>();

    protected Guid CurrentUserID => GetUserID();

    protected Guid GetUserID()
    {
        var userID = HttpContext.User.Claims.FirstOrDefault(a => a.Type == "id");
        if (userID is null)
        {
            return Guid.Empty;
        }
        return new Guid(userID.Value);
    }

    public bool IsAdmin => IsInRole(Roles.ADMIN);
    public bool IsManager => IsInRole(Roles.MANAGER);
    public bool IsTechnicalStaff => IsInRole(Roles.TECHNICAL);
    public bool IsSaleStaff => IsInRole(Roles.SALE);
    public bool IsCustomer => IsInRole(Roles.CUSTOMER);
    private bool IsInRole(string role) {
        return User.IsInRole(role);
    }
}
