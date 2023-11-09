using API.DTOs.Requests.ServicePacks;
using API.Services.Interfaces;
using Domain.Constants;
using Domain.Exceptions;
using Domain.Models.Contracts;
using Google.Api.Gax.Grpc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Helpers;

namespace API.Controllers;

[Route("/v1/itsds/servicepack")]
public class ServicePackController : BaseController
{
    private readonly IServicePackService _servicePackService;

    public ServicePackController(IServicePackService servicePackService)
    {
        _servicePackService = servicePackService;
    }

    [Authorize]
    [HttpGet("all")]

    public async Task<IActionResult> GetAllServicePack()
    {
        return Ok(await _servicePackService.Get());
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetServicePack(
    [FromQuery] string? filter,
    [FromQuery] string? sort,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 5)
    {
        var result = await _servicePackService.Get();
        var pagedResponse = result.AsQueryable().GetPagedData(page, pageSize, filter, sort);
        return Ok(pagedResponse);
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetServicePackById(int id)
    {
        try
        {
            var result = await _servicePackService.GetById(id);
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
    public async Task<IActionResult> CreateServicePack([FromBody] CreateServicePackRequest model)
    {
        try
        {
            var result = await _servicePackService.Create(model);
            return Ok(new { Message = "Service Pack Added Successfully", Data = result });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateServicePack(int id, [FromBody] UpdateServicePackRequest model)
    {
        try
        {
            var result = await _servicePackService.Update(id, model);
            return Ok(new { Message = "Service Pack Updated Successfully", Data = result });
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
    public async Task<IActionResult> DeleteServicePack(int id)
    {
        try
        {
            await _servicePackService.Remove(id);
            return Ok("Service Pack Removed Successfully");
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
