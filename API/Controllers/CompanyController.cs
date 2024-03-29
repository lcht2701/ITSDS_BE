﻿using API.DTOs.Requests.Companies;
using API.Services.Interfaces;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Helpers;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

[Route("/v1/itsds/company")]
public class CompanyController : BaseController
{
    private readonly ICompanyService _companyService;
    private readonly ICompanyAddressService _companyAddressService;

    public CompanyController(ICompanyService companyService, ICompanyAddressService companyAddressService)
    {
        _companyService = companyService;
        _companyAddressService = companyAddressService;
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

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.ACCOUNTANT}")]
    [HttpGet]
    [SwaggerResponse(200, "Get List Company", typeof(List<Domain.Models.Contracts.Company>))]
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
            int totalPage = (int)Math.Ceiling((double)companies.Count / pageSize);
            return Ok(new { TotalPage = totalPage, Data = pagedResponse });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.ACCOUNTANT}")]
    [HttpGet("{id}")]
    [SwaggerResponse(200, "Get Company By Id", typeof(List<Domain.Models.Contracts.Company>))]
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
            await _companyAddressService.RemoveByCompany(id);
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
