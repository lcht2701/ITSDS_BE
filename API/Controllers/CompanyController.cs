using API.DTOs.Requests.Companies;
using API.Services.Interfaces;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Helpers;

namespace API.Controllers;

[Route("/v1/itsds/company")]
public class CompanyController : BaseController
{
    private readonly ICompanyService _companyService;
    private readonly IServiceContractService _serviceContractService;

    public CompanyController(ICompanyService companyService, IServiceContractService serviceContractService)
    {
        _companyService = companyService;
        _serviceContractService = serviceContractService;
    }

    [Authorize]
    [HttpGet("all")]

    public async Task<IActionResult> GetAll()
    {
        try
        {
            return Ok((await _companyService.Get()).Where(x => x.IsActive == true));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
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

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.ACCOUNTANT}")]
    [HttpGet]
    public async Task<IActionResult> Get(
    [FromQuery] string? filter,
    [FromQuery] string? sort,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 5)
    {
        try
        {
            var companies = await _companyService.Get();
            var pagedResponse = companies.AsQueryable().GetPagedData(page, pageSize, filter, sort);
            return Ok(pagedResponse);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.ACCOUNTANT}")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var result = await _companyService.GetById(id);
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

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.ACCOUNTANT}")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCompanyRequest model)
    {
        try
        {
            var result = await _companyService.Create(model);
            return Ok(new { Message = "Company Created Successfully", Data = result });

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.ACCOUNTANT}")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCompanyRequest model)
    {
        try
        {
            var result = await _companyService.Update(id, model);
            return Ok(new { Message = "Company Updated Successfully", Data = result });
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

    [Authorize(Roles = $"{Roles.MANAGER}")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _companyService.Remove(id);
            return Ok("Company Removed Successfully");
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
