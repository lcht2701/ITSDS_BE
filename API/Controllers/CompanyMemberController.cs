using API.DTOs.Requests.CompanyMembers;
using API.DTOs.Responses.Contracts;
using API.Services.Implements;
using API.Services.Interfaces;
using Domain.Constants;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Helpers;
using Persistence.Repositories.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

[Route("/v1/itsds/company/member/")]
public class CompanyMemberController : BaseController
{
    private readonly ICompanyMemberService _companyMemberService;
    private readonly IFirebaseService _firebaseService;
    private readonly IRepositoryBase<User> _userRepository;

    public CompanyMemberController(ICompanyMemberService companyMemberService, IFirebaseService firebaseService, IRepositoryBase<User> userRepository)
    {
        _companyMemberService = companyMemberService;
        _firebaseService = firebaseService;
        _userRepository = userRepository;
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.ACCOUNTANT},{Roles.CUSTOMER}")]
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] string? filter,
        [FromQuery] string? sort,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5)
    {
        try
        {
            var result = await _companyMemberService.Get(CurrentUserID);
            var pagedResponse = result.AsQueryable().GetPagedData(page, pageSize, filter, sort);
            int totalPage = (int)Math.Ceiling((double)result.Count / pageSize);
            return Ok(new { TotalPage = totalPage, Data = pagedResponse });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.ACCOUNTANT},{Roles.CUSTOMER}")]
    [HttpGet("company-admins")]
    public async Task<IActionResult> GetCompanyAdmins(int companyId)
    {
        try
        {
            var result = await _companyMemberService.GetCompanyAdmins(companyId);
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

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.ACCOUNTANT},{Roles.CUSTOMER}")]
    [HttpGet("select-list")]
    [SwaggerResponse(200, "Get Select List", typeof(List<Domain.Models.User>))]
    public async Task<IActionResult> GetSelectList(int companyId)
    {
        try
        {
            var members = await _companyMemberService.GetMemberNotInCompany(companyId);
            return Ok(members);
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

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.ACCOUNTANT},{Roles.CUSTOMER}")]
    [HttpGet("{id}")]
    [SwaggerResponse(200, "Get Category by Id", typeof(CompanyMember))]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        try
        {
            var result = await _companyMemberService.GetById(id);
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

    [Authorize(Roles = Roles.CUSTOMER)]
    [HttpPost()]
    public async Task<IActionResult> Add([FromBody] AddCompanyMemberRequest model)
    {
        try
        {
            var result = await _companyMemberService.Add(model, CurrentUserID);
            return Ok(new { Message = "Member Added Successfully", Data = result });
        }
        catch (UnauthorizedException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (BadRequestException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = Roles.CUSTOMER)]
    [HttpPut("{memberId}")]
    public async Task<IActionResult> UpdateTeamMember(int memberId, [FromBody] UpdateCompanyMemberRequest model)
    {
        try
        {
            var result = await _companyMemberService.Update(memberId, model, CurrentUserID);
            return Ok(new { Message = "Member Updated Successfully", Data = result });
        }
        catch (UnauthorizedException ex)
        {
            return Unauthorized(ex.Message);
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

    [Authorize(Roles = Roles.CUSTOMER)]
    [HttpDelete("{memberId}")]
    public async Task<IActionResult> RemoveTeamMember(int memberId)
    {
        try
        {
            await _companyMemberService.Remove(memberId, CurrentUserID);
            return Ok("Removed Successfully");
        }
        catch (UnauthorizedException ex)
        {
            return Unauthorized(ex.Message);
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