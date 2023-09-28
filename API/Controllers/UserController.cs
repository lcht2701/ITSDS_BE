using API.DTOs.Auth.Requests;
using API.DTOs.Users.Requests;
using AutoMapper;
using CloudinaryDotNet.Actions;
using Domain.Constants;
using Domain.Exceptions;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Persistence.Repositories.Interfaces;
using Persistence.Services.Interfaces;

namespace API.Controllers;

[Route("/v1/itsds/user")]
public class UserController : BaseController
{
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IPhotoService _photoService;

    public UserController(IRepositoryBase<User> userRepository, IPhotoService photoService)
    {
        _userRepository = userRepository;
        _photoService = photoService;
    }

    [Authorize(Roles = Roles.ADMIN)]
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        return Ok(await _userRepository.ToListAsync());
    }

    //[Authorize(Roles = Roles.ADMIN)]
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest model)
    {

        User entity = Mapper.Map(model, new User());
        //Hash password
        var passwordHasher = new PasswordHasher<User>();
        entity.Password = passwordHasher.HashPassword(entity, model.Password);
        entity.isActive = true;
        await _userRepository.CreateAsync(entity);
        return Ok("Created Successfully");
    }


    [Authorize(Roles = Roles.ADMIN)]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest req)
    {
        var target = await _userRepository.FoundOrThrow(c => c.Id.Equals(id), new NotFoundException());
        User entity = Mapper.Map(req, target);
        await _userRepository.UpdateAsync(entity);
        return Accepted("Updated Successfully");
    }

    [Authorize(Roles = Roles.ADMIN)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var target = await _userRepository.FoundOrThrow(c => c.Id.Equals(id), new NotFoundException());
        //Soft Delete
        await _userRepository.DeleteAsync(target);
        return Ok("Deleted Successfully");
    }

    [Authorize]
    [HttpGet("{id}/profile")]
    public async Task<IActionResult> GetProfile()
    {
        var user = await _userRepository.FoundOrThrow(u => u.Id.Equals(CurrentUserID), new NotFoundException("User is not found"));
        return Ok(user);
    }

    [Authorize]
    [HttpPatch("{id}/update-profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest req)
    {
        var target = await _userRepository.FoundOrThrow(c => c.Id.Equals(CurrentUserID), new NotFoundException("User is not found"));
        User entity = Mapper.Map(req, target);
        await _userRepository.UpdateAsync(entity);
        return Accepted("Updated Successfully");
    }

    [Authorize]
    [HttpPatch("{id}/uploadAvatar")]
    public async Task<IActionResult> UploadAvatarProfile(IFormFile file)
    {
        var user = await _userRepository.FoundOrThrow(c => c.Id.Equals(CurrentUserID), new NotFoundException("User is not found"));
        var result = await _photoService.AddPhotoAsync(file);
        user.AvatarUrl = result.Url.ToString();
        await _userRepository.UpdateAsync(user);
        return Accepted("Updated Successfully");
    }
}
