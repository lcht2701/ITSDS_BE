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

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.ADMIN}")]
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
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest model)
    {
        try
        {
            var user = await _userService.Create(model);
            await _userService.CreateUserDocument(user);
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
            await _userService.UpdateUserDocument(user);
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
            await _userService.UpdateUserDocument(user);
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
            await _userService.UpdateUserDocument(user);
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