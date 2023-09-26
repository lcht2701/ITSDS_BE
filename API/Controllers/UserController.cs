using API.DTOs.Users.Requests;
using AutoMapper;
using Domain.Constants;
using Domain.Exceptions;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Persistence.Repositories.Interfaces;

namespace API.Controllers;

[Route("/v1/itsds/user")]
public class UserController : BaseController
{
    private readonly IRepositoryBase<User> _userRepository;

    public UserController(IRepositoryBase<User> userRepository)
    {
        _userRepository = userRepository;
    }

    [Authorize(Roles = Roles.ADMIN)]
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        return Ok(await _userRepository.ToListAsync());
    }

    [Authorize(Roles = Roles.ADMIN)]
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest model)
    {

        User entity = Mapper.Map(model, new User());
        //Hash password
        var passwordHasher = new PasswordHasher<User>();
        entity.Password = passwordHasher.HashPassword(entity, model.Password);
        entity.isActive = true;
        await _userRepository.CreateAsync(entity);
        return StatusCode(StatusCodes.Status201Created);
    }


    [Authorize(Roles = Roles.ADMIN)]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest req)
    {
        var target = await _userRepository.FoundOrThrow(c => c.Id.Equals(id), new NotFoundException());
        User entity = Mapper.Map(req, target);
        await _userRepository.UpdateAsync(entity);
        return StatusCode(StatusCodes.Status204NoContent);
    }

    

    [Authorize(Roles = Roles.ADMIN)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var target = await _userRepository.FoundOrThrow(c => c.Id.Equals(id), new NotFoundException());
        //Soft Delete
        await _userRepository.DeleteAsync(target);
        return StatusCode(StatusCodes.Status204NoContent);
    }
}
