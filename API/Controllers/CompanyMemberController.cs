using API.DTOs.Requests.CompanyMembers;
using API.Services.Interfaces;
using Domain.Constants;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("/v1/itsds/company/member/")]
public class CompanyMemberController : BaseController
{
    private readonly ICompanyMemberService _companyMemberService;

    public CompanyMemberController(ICompanyMemberService companyMemberService)
    {
        _companyMemberService = companyMemberService;
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.ACCOUNTANT}")]
    [HttpGet("{companyId}")]
    public async Task<IActionResult> GetTeamMembersByTeam(int companyId)
    {
        try
        {
            var members = await _companyMemberService.Get(companyId);
            return Ok(members);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Company is not exist");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.ACCOUNTANT}")]
    [HttpGet("select-list")]
    public async Task<IActionResult> GetSelectList(int companyId)
    {
        try
        {
            var members = await _companyMemberService.GetMemberNotInCompany(companyId);
            return Ok(members);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Company is not exist");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [Authorize(Roles = $"{Roles.MANAGER},{Roles.ACCOUNTANT}")]
    [HttpPost()]
    public async Task<IActionResult> Add([FromBody] AddCompanyMemberRequest model)
    {
        try
        {
            var result = await _companyMemberService.Add(model);
            return Ok(new { Message = "Member Added Successfully", Data = result });
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

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.ACCOUNTANT}")]
    [HttpPut("{memberId}")]
    public async Task<IActionResult> UpdateTeamMember(int memberId, [FromBody] UpdateCompanyMemberRequest model)
    {
        try
        {
            var result = await _companyMemberService.Update(memberId, model);
            return Ok(new { Message = "Member Updated Successfully", Data = result });
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

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.ACCOUNTANT}")]
    [HttpDelete("{memberId}")]
    public async Task<IActionResult> RemoveTeamMember(int memberId)
    {
        try
        {
            await _companyMemberService.Remove(memberId);
            return Ok("Removed Successfully");
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