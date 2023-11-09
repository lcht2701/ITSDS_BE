using API.Services.Interfaces;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Helpers;

namespace API.Controllers;

[Route("/v1/itsds/servicepack/services")]
public class ServiceServicePackController : BaseController
{
    private readonly IServiceServicePackService _sspService;

    public ServiceServicePackController(IServiceServicePackService sspService)
    {
        _sspService = sspService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetServices(int packId,
    [FromQuery] string? filter,
    [FromQuery] string? sort,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 5)
    {
        var result = await _sspService.GetServices(packId);
        var pagedResponse = result.AsQueryable().GetPagedData(page, pageSize, filter, sort);
        return Ok(pagedResponse);
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpPost]
    public async Task<IActionResult> AddServiceToServicePack(int packId, List<int> serviceIds)
    {
        try
        {
            await _sspService.AddService(packId, serviceIds);
            return Ok("Add Services Successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveServiceFromServicePack(int id)
    {
        try
        {
            await _sspService.RemoveService(id);
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
