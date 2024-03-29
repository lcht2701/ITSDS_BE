﻿
using API.DTOs.Requests.Services;
using API.Services.Interfaces;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Helpers;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

[Route("/v1/itsds/service")]
public class ServiceController : BaseController
{
    private readonly IServiceService _serviceService;

    public ServiceController(IServiceService serviceService)
    {
        _serviceService = serviceService;
    }

    [Authorize]
    [HttpGet("all")]

    public async Task<IActionResult> GetAllService()
    {
        var result = await _serviceService.Get();
        return Ok(result);
    }

    [Authorize]
    [HttpGet]
    [SwaggerResponse(200, "Get Service", typeof(List<Domain.Models.Contracts.Service>))]
    public async Task<IActionResult> GetServices(
    [FromQuery] string? filter,
    [FromQuery] string? sort,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 5)
    {
        var result = await _serviceService.Get();
        var pagedResponse = result.AsQueryable().GetPagedData(page, pageSize, filter, sort);
        int totalPage = (int)Math.Ceiling((double)result.Count / pageSize);
        return Ok(new { TotalPage = totalPage, Data = pagedResponse }); 
    }

    [Authorize]
    [HttpGet("category")]
    [SwaggerResponse(200, "Get Service By Category", typeof(List<Domain.Models.Contracts.Service>))]
    public async Task<IActionResult> GetByCategory(int categoryId)
    {
        try
        {
            var result = await _serviceService.GetByCategory(categoryId);
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
    [HttpGet("{id}")]
    [SwaggerResponse(200, "Get Service By Id", typeof(Domain.Models.Contracts.Service))]
    public async Task<IActionResult> GetServiceById(int id)
    {
        try
        {
            var result = await _serviceService.GetById(id);
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
    public async Task<IActionResult> CreateService([FromBody] CreateServiceRequest model)
    {
        try
        {
            var result = await _serviceService.Create(model);
            return Ok(new { Message = "Service Created Successfully", Data = result });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateService(int id, [FromBody] UpdateServiceRequest model)
    {
        try
        {
            var result = await _serviceService.Update(id, model);
            return Ok(new { Message = "Service Updated Successfully", Data = result });
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
    public async Task<IActionResult> DeleteService(int id)
    {
        try
        {
            await _serviceService.Remove(id);
            return Ok("Service Removed Successfully");
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
