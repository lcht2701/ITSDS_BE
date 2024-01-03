using API.DTOs.Requests.Users;
using API.Services.Interfaces;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Helpers;

namespace API.Controllers;

[Route("/v1/itsds/user")]
public class UserController : BaseController
{
    private readonly IUserService _userService;
    private readonly IServiceContractService _serviceContractService;
    private readonly IFirebaseService _firebaseService;

    public UserController(IUserService userService, IServiceContractService serviceContractService, IFirebaseService firebaseService)
    {
        _userService = userService;
        _serviceContractService = serviceContractService;
        _firebaseService = firebaseService;
    }

    [Authorize]
    [HttpGet("active-services")]

    public async Task<IActionResult> GetActiveServices()
    {
        try
        {
            return Ok(await _serviceContractService.GetActiveServicesOfMemberCompany(CurrentUserID));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    #region Selection List By Roles
    [Authorize(Roles = Roles.ITSDSEmployees)]
    [HttpGet("list/managers")]
    public async Task<IActionResult> GetManagers()
    {
        try
        {
            var result = await _userService.GetManagers();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = Roles.ITSDSEmployees)]
    [HttpGet("list/accountants")]
    public async Task<IActionResult> GetAccountants()
    {
        try
        {
            var result = await _userService.GetAccountants();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = Roles.ITSDSEmployees)]
    [HttpGet("list/customers")]
    public async Task<IActionResult> GetCustomers()
    {
        try
        {
            var result = await _userService.GetCustomers();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = Roles.ITSDSEmployees)]
    [HttpGet("list/admins")]
    public async Task<IActionResult> GetAdmins()
    {
        try
        {
            var result = await _userService.GetAdmins();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = Roles.ITSDSEmployees)]
    [HttpGet("list/technicians")]
    public async Task<IActionResult> GetTechnicians()
    {
        try
        {
            var result = await _userService.GetTechnicians();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    #endregion

    [Authorize]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            var result = await _userService.Get();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.ADMIN}")]
    [HttpGet]
    public async Task<IActionResult> GetUsers(
    [FromQuery] string? filter,
    [FromQuery] string? sort,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 5)
    {
        try
        {
            var result = await _userService.Get();
            var pagedResponse = result.AsQueryable().GetPagedData(page, pageSize, filter, sort);
            return Ok(pagedResponse);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        try
        {
            var result = await _userService.GetById(id);
            return Ok(result);
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

    [Authorize(Roles = Roles.ADMIN)]
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserOrCustomerAdmin model)
    {
        try
        {
            var user = await _userService.Create(model);
            if (user != null && await _firebaseService.CreateFirebaseUser(model.UserModel.Email, model.UserModel.Password) == true)
            {
                await _firebaseService.CreateUserDocument(user!);
            }
            return Ok("Created Successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [Authorize(Roles = Roles.ADMIN)]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest model)
    {
        try
        {
            var user = await _userService.Update(id, model);
            if (user != null && await _firebaseService.UpdateFirebaseUser(model.Email, null) == true)
            {
                await _firebaseService.CreateUserDocument(user!);
            }
            await _firebaseService.UpdateUserDocument(user);
            return Ok("Updated Successfully");
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

    [Authorize(Roles = Roles.ADMIN)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            await _userService.Remove(id);
            return Ok("Deleted Successfully");
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

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var result = await _userService.GetProfile(CurrentUserID);
            return Ok(result);
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

    [Authorize]
    [HttpPatch("update-profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest model)
    {
        try
        {
            var user = await _userService.UpdateProfile(CurrentUserID, model);
            await _firebaseService.UpdateUserDocument(user);
            return Ok("Updated Successfully");
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

    [Authorize]
    [HttpPatch("uploadAvatarFirebase")]
    public async Task<IActionResult> UploadImageFirebase(IFormFile file)
    {
        try
        {
            var result = await _userService.UploadImageFirebase(CurrentUserID, file);
            return Ok(result);
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

    [Authorize]
    [HttpPatch("uploadAvatarByUrl")]
    public async Task<IActionResult> UploadAvatarByUrl([FromBody] UpdateAvatarUrlRequest model)
    {
        try
        {
            var user = await _userService.UploadAvatarByUrl(CurrentUserID, model);
            await _firebaseService.UpdateUserDocument(user);
            return Ok("Avatar URL updated successfully");
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