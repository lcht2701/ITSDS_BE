using API.DTOs.Requests.Users;
using API.DTOs.Responses.Users;
using AutoMapper;
using CloudinaryDotNet.Actions;
using Domain.Constants;
using Domain.Exceptions;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Persistence.Repositories.Interfaces;
using Persistence.Services.Interfaces;
using System.Web;
using static System.Net.Mime.MediaTypeNames;

namespace API.Controllers;

[Route("/v1/itsds/user")]
public class UserController : BaseController
{
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IPhotoService _photoService;
    private readonly IFirebaseStorageService _firebaseStorageService;

    public UserController(IRepositoryBase<User> userRepository, IPhotoService photoService, IFirebaseStorageService firebaseStorageService)
    {
        _userRepository = userRepository;
        _photoService = photoService;
        _firebaseStorageService = firebaseStorageService;
    }

    [Authorize(Roles = Roles.ADMIN)]
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var result = await _userRepository.ToListAsync();
        return Ok(result);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var result = await _userRepository.FoundOrThrow(u => u.Id.Equals(id), new NotFoundException("User is not found"));
        return Ok(result);
    }

    //Enable back to test authorization
    //[Authorize(Roles = Roles.ADMIN)]
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest model)
    {

        User entity = Mapper.Map(model, new User());
        //Hash password
        var passwordHasher = new PasswordHasher<User>();
        entity.Password = passwordHasher.HashPassword(entity, model.Password);
        entity.IsActive = true;
        await _userRepository.CreateAsync(entity);
        return Ok("Created Successfully");
    }


    [Authorize(Roles = Roles.ADMIN)]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest req)
    {
        var target = await _userRepository.FoundOrThrow(c => c.Id.Equals(id), new NotFoundException("User not found"));
        User entity = Mapper.Map(req, target);
        await _userRepository.UpdateAsync(entity);
        return Ok("Updated Successfully");
    }

    [Authorize(Roles = Roles.ADMIN)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var target = await _userRepository.FoundOrThrow(c => c.Id.Equals(id), new NotFoundException("User not found"));
        //Soft Delete
        await _userRepository.DeleteAsync(target);
        return Ok("Deleted Successfully");
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var user = await _userRepository.FoundOrThrow(u => u.Id.Equals(CurrentUserID), new NotFoundException("User is not found"));
        return Ok(user);
    }

    [Authorize]
    [HttpPatch("update-profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest req)
    {
        var target = await _userRepository.FoundOrThrow(c => c.Id.Equals(CurrentUserID), new NotFoundException("User is not found"));
        User entity = Mapper.Map(req, target);
        await _userRepository.UpdateAsync(entity);
        return Ok("Updated Successfully");
    }

    [Authorize]
    [HttpPatch("update-profile-with-avatar")]
    public async Task<IActionResult> UpdateProfileWithAvatar([FromBody] UpdateProfileRequest req)
    {
        var target = await _userRepository.FoundOrThrow(c => c.Id.Equals(CurrentUserID), new NotFoundException("User is not found"));
        User entity = Mapper.Map(req, target);
        await _userRepository.UpdateAsync(entity);
        return Ok("Updated Successfully");
    }

    //[Authorize]
    //[HttpPatch("uploadAvatarCloudinary")]
    //public async Task<IActionResult> UploadAvatarProfileCloudinary(IFormFile file)
    //{
    //    var user = await _userRepository.FoundOrThrow(c => c.Id.Equals(CurrentUserID), new NotFoundException("User is not found"));
    //    var result = await _photoService.AddPhotoAsync(file);
    //    user.AvatarUrl = result.Url.ToString();
    //    await _userRepository.UpdateAsync(user);
    //    return Ok(user.AvatarUrl);
    //}

    [Authorize]
    [HttpPatch("uploadAvatarFirebase")]
    public async Task<IActionResult> UploadAvatarProfileFirebase(IFormFile file)
    {
        var user = await _userRepository.FoundOrThrow(c => c.Id.Equals(CurrentUserID), new NotFoundException("User is not found"));
        if (file == null || file.Length == 0)
        {
            throw new BadRequestException("No file uploaded.");
        }

        var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Position = 0;

        var linkImage = await _firebaseStorageService.UploadImageFirebaseAsync(stream, file.FileName);
        user.AvatarUrl = linkImage;
        await _userRepository.UpdateAsync(user);
        return Ok(linkImage);
      
    }

}
