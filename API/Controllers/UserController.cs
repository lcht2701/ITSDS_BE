using API.DTOs.Requests.Users;
using API.DTOs.Responses.Users;
using Domain.Constants;
using Domain.Exceptions;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Persistence.Helpers;
using Persistence.Repositories.Interfaces;
using Persistence.Services.Interfaces;

namespace API.Controllers;

[Route("/v1/itsds/user")]
public class UserController : BaseController
{
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IFirebaseStorageService _firebaseStorageService;

    public UserController(IRepositoryBase<User> userRepository, IFirebaseStorageService firebaseStorageService)
    {
        _userRepository = userRepository;
        _firebaseStorageService = firebaseStorageService;
    }

    [Authorize(Roles = Roles.ADMIN)]
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var result = await _userRepository.ToListAsync();
        var response = new List<GetUserResponse>();
        foreach (var user in result)
        {
            var entity = Mapper.Map(user, new GetUserResponse());
            entity.Role = EnumExtensions.GetEnumDescription(user.Role!);
            entity.Gender = EnumExtensions.GetEnumDescription(user.Gender!);

            entity.DateOfBirth = (entity.DateOfBirth != DateTime.MinValue) ? entity.DateOfBirth : null;
            entity.CreatedAt = (entity.CreatedAt != DateTime.MinValue) ? entity.CreatedAt : null;
            entity.ModifiedAt = (entity.ModifiedAt != DateTime.MinValue) ? entity.ModifiedAt : null;
            entity.DeletedAt = (entity.DeletedAt != DateTime.MinValue) ? entity.DeletedAt : null;

            response.Add(entity);
        }

        return Ok(response);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var result =
            await _userRepository.FoundOrThrow(u => u.Id.Equals(id), new NotFoundException("User is not found"));

        var entity = Mapper.Map(result, new GetUserResponse());
        entity.Role = EnumExtensions.GetEnumDescription(result.Role!);
        entity.Gender = EnumExtensions.GetEnumDescription(result.Gender!);

        entity.DateOfBirth = (entity.DateOfBirth != DateTime.MinValue) ? entity.DateOfBirth : null;
        entity.CreatedAt = (entity.CreatedAt != DateTime.MinValue) ? entity.CreatedAt : null;
        entity.ModifiedAt = (entity.ModifiedAt != DateTime.MinValue) ? entity.ModifiedAt : null;
        entity.DeletedAt = (entity.DeletedAt != DateTime.MinValue) ? entity.DeletedAt : null;

        return Ok(entity);
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
        var result = await _userRepository.FoundOrThrow(u => u.Id.Equals(CurrentUserID),
            new NotFoundException("User is not found"));
        var entity = Mapper.Map(result, new GetUserProfileResponse());
        entity.Role = EnumExtensions.GetEnumDescription(result.Role!);
        entity.Gender = EnumExtensions.GetEnumDescription(result.Gender!);

        entity.DateOfBirth = (entity.DateOfBirth != DateTime.MinValue) ? entity.DateOfBirth : null;

        return Ok(entity);
    }

    [Authorize]
    [HttpPatch("update-profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest req)
    {
        var target = await _userRepository.FoundOrThrow(c => c.Id.Equals(CurrentUserID),
            new NotFoundException("User is not found"));
        User entity = Mapper.Map(req, target);
        await _userRepository.UpdateAsync(entity);
        return Ok("Updated Successfully");
    }

    [Authorize]
    [HttpPatch("update-profile-with-avatar")]
    public async Task<IActionResult> UpdateProfileWithAvatar([FromBody] UpdateProfileRequest req)
    {
        var target = await _userRepository.FoundOrThrow(c => c.Id.Equals(CurrentUserID),
            new NotFoundException("User is not found"));
        User entity = Mapper.Map(req, target);
        await _userRepository.UpdateAsync(entity);
        return Ok("Updated Successfully");
    }

    [Authorize]
    [HttpPatch("uploadAvatarFirebase")]
    public async Task<IActionResult> UploadImageFirebase(IFormFile file)
    {
        var user = await _userRepository.FoundOrThrow(c => c.Id.Equals(CurrentUserID),
            new NotFoundException("User is not found"));
        if (file == null || file.Length == 0)
        {
            throw new BadRequestException("No file uploaded.");
        }

        var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Position = 0;

        var linkImage = await _firebaseStorageService.UploadFirebaseAsync(stream, file.FileName);
        user.AvatarUrl = linkImage;
        await _userRepository.UpdateAsync(user);
        return Ok(linkImage);
    }

    [Authorize]
    [HttpGet("current-user")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var result = await _userRepository.FirstOrDefaultAsync(u => u.Id.Equals(CurrentUserID));
        var entity = Mapper.Map(result, new GetUserResponse());
        entity.Role = EnumExtensions.GetEnumDescription(result.Role!);
        entity.Gender = EnumExtensions.GetEnumDescription(result.Gender!);

        entity.DateOfBirth = (entity.DateOfBirth != DateTime.MinValue) ? entity.DateOfBirth : null;
        entity.CreatedAt = (entity.CreatedAt != DateTime.MinValue) ? entity.CreatedAt : null;
        entity.ModifiedAt = (entity.ModifiedAt != DateTime.MinValue) ? entity.ModifiedAt : null;
        entity.DeletedAt = (entity.DeletedAt != DateTime.MinValue) ? entity.DeletedAt : null;

        return Ok(entity);
    }
}