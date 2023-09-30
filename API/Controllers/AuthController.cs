using Domain.Exceptions;
using Domain.Constants;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Persistence.Repositories.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.DTOs.Requests.Auths;
using API.DTOs.Responses.Auths;

namespace API.Controllers;

[Route("/v1/itsds/auth")]
public class AuthController : BaseController
{
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IConfiguration _configuration;

    public AuthController(IRepositoryBase<User> userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        var user = await _userRepository.FirstOrDefaultAsync(x => x.Username.Equals(model.Username));
        LoginResponse loginResponse = new();
        if (user == null)
        {
            throw new NotFoundException("User is not found");
        }
        var passwordHasher = new PasswordHasher<User>();
        var isMatchPassword = passwordHasher.VerifyHashedPassword(user, user.Password, model.Password) == PasswordVerificationResult.Success;
        if (!isMatchPassword)
        {
            throw new UnauthorizedException("Password is incorrect");
        }
        loginResponse.User = user;
        loginResponse.AccessToken = GenerateToken(user);
        SetCookie(ConstantItems.ACCESS_TOKEN, loginResponse.AccessToken);
        return Ok(loginResponse);
    }

    [HttpPost("login-admin")]
    public async Task<IActionResult> LoginAdmin([FromBody] LoginRequest model)
    {
        var user = await _userRepository.FirstOrDefaultAsync(x => x.Username.Equals(model.Username));
        LoginResponse loginResponse = new();

        if (user == null)
        {
            throw new NotFoundException("User is not found");
        }

        if (IsAdmin)
        {
            throw new UnauthorizedException("You are not allowed to enter");
        }

        var passwordHasher = new PasswordHasher<User>();
        var isMatchPassword = passwordHasher.VerifyHashedPassword(user, user.Password, model.Password) == PasswordVerificationResult.Success;

        if (!isMatchPassword)
        {
            throw new UnauthorizedException("Password is incorrect");
        }

        loginResponse.User = user;
        loginResponse.AccessToken = GenerateToken(user);
        SetCookie(ConstantItems.ACCESS_TOKEN, loginResponse.AccessToken);
        return Ok(loginResponse);
    }

    [Authorize]
    [HttpPatch("{id}/change-password")]
    public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest req)
    {
        var user = await _userRepository.FoundOrThrow(u => u.Id.Equals(id), new NotFoundException("User is not found"));

        var passwordHasher = new PasswordHasher<User>();
        var isMatchPassword = passwordHasher.VerifyHashedPassword(user, user.Password, req.CurrentPassword) == PasswordVerificationResult.Success;
        if (!isMatchPassword)
        {
            throw new BadRequestException("Your current password is incorrect.");
        }
        if (req.NewPassword.Equals(req.CurrentPassword))
        {
            throw new BadRequestException("New password should not be the same as old password.");
        }
        if (!req.NewPassword.Equals(req.ConfirmNewPassword))
        {
            throw new BadRequestException("Password and Confirm Password does not match.");
        }
        user.Password = passwordHasher.HashPassword(user, req.NewPassword);

        await _userRepository.UpdateAsync(user);
        return Accepted("Updated Successfully");
    }

    //[HttpPost("logout")]
    //public IActionResult Logout()
    //{
    //    RemoveCookie(ConstantItems.ACCESS_TOKEN);
    //    return Ok();
    //}

    #region Generate JWT Token
    private string GenerateToken(User user)
    {
        var claims = new[] {
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };
        //Remember to change back to 2 hours
        //return new JwtSecurityTokenHandler().WriteToken(
        //    GenerateTokenByClaims(claims, DateTime.Now.AddMinutes(120))
        //    );
        return new JwtSecurityTokenHandler().WriteToken(
            GenerateTokenByClaims(claims, DateTime.Now.AddDays(1))
            );
    }

    private SecurityToken GenerateTokenByClaims(Claim[] claims, DateTime expires)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        return new JwtSecurityToken(_configuration["JWT:Issuer"],
             _configuration["JWT:Audience"],
             claims,
             expires: expires,
             signingCredentials: credentials);
    }
    #endregion

    #region Handle Cookie
    private void RemoveCookie(string key)
    {
        HttpContext.Response.Cookies.Delete(key);
    }

    private void SetCookie(string key, string value)
    {
        CookieOptions cookieOptions = new CookieOptions
        {
            Expires = DateTime.Now.AddDays(2),
            IsEssential = true,
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
        };
        Response.Cookies.Append(key, value, cookieOptions);
    }
    #endregion
}
