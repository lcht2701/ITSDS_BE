using API.DTOs.Requests.CompanyAddresss;
using API.Services.Interfaces;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Helpers;

namespace API.Controllers;

[Route("/v1/itsds/company/companyAddress")]
public class CompanyAddressController : BaseController
{
    private readonly ICompanyAddressService _CompanyAddressService;

    public CompanyAddressController(ICompanyAddressService CompanyAddressService)
    {
        _CompanyAddressService = CompanyAddressService;
    }

    [Authorize]
    [HttpGet("{companyId}/select-list")]
    public async Task<IActionResult> GetAll(int companyId)
    {
        var result = await _CompanyAddressService.Get(companyId);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("{companyId}/list")]
    public async Task<IActionResult> Get(
    int companyId,
    [FromQuery] string? filter,
    [FromQuery] string? sort,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 5)
    {
        var result = await _CompanyAddressService.Get(companyId);
        var pagedResponse = result.AsQueryable().GetPagedData(page, pageSize, filter, sort);
        return Ok(pagedResponse);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var result = await _CompanyAddressService.GetById(id);
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

    [Authorize(Roles = Roles.MANAGER)]
    [HttpPost]
    public async Task<IActionResult> Create(int companyId, [FromBody] CreateCompanyAddressRequest model)
    {
        try
        {
            var entity = await _CompanyAddressService.Create(companyId, model);
            return Ok(new
            {
                Message = "CompanyAddress Created Successfully",
                Data = entity
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCompanyAddressRequest model)
    {
        try
        {
            var entity = await _CompanyAddressService.Update(id, model);
            return Ok(new
            {
                Message = "CompanyAddress Updated Successfully",
                Data = entity
            });
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

    [Authorize(Roles = Roles.MANAGER)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(int id)
    {
        try
        {
            await _CompanyAddressService.Remove(id);
            return Ok("CompanyAddress Removed Successfully");
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
