﻿
using API.DTOs.Requests.Contracts;
using API.DTOs.Responses.Contracts;
using API.Services.Interfaces;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Helpers;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

[Route("/v1/itsds/contract")]
public class ContractController : BaseController
{
    private readonly IContractService _contractService;
    private readonly IServiceContractService _serviceContractService;

    public ContractController(IContractService contractService, IServiceContractService serviceContractService)
    {
        _contractService = contractService;
        _serviceContractService = serviceContractService;
    }

    [Authorize]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllContract()
    {
        var result = await _contractService.Get();
        return Ok(result);
    }

    [Authorize]
    [HttpGet]
    [SwaggerResponse(200, "Get List Contracts", typeof(List<GetContractResponse>))]
    public async Task<IActionResult> GetContracts(
    [FromQuery] string? filter,
    [FromQuery] string? sort,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 5)
    {
        var result = await _contractService.Get();
        var pagedResponse = result.AsQueryable().GetPagedData(page, pageSize, filter, sort);
        return Ok(pagedResponse);
    }

    [Authorize(Roles = Roles.CUSTOMER)]
    [HttpGet("customer")]
    [SwaggerResponse(200, "Get Contract By Company Admin", typeof(List<GetContractResponse>))]
    public async Task<IActionResult> GetByCustomer()
    {
        try
        {
            var result = await _contractService.GetByCompanyAdmin(CurrentUserID);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.ACCOUNTANT},{Roles.CUSTOMER}")]
    [HttpGet("{id}")]
    [SwaggerResponse(200, "Get Contract By Id", typeof(GetContractResponse))]
    public async Task<IActionResult> GetContractById(int id)
    {
        try
        {
            var result = await _contractService.GetById(id, CurrentUserID);
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
    public async Task<IActionResult> CreateContract([FromBody] CreateContractRequest model)
    {
        try
        {
            var result = await _contractService.Create(model);
            return Ok(new { Message = "Contract Created Successfully", Data = result });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.ACCOUNTANT}")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateContract(int id, [FromBody] UpdateContractRequest model)
    {
        try
        {
            var result = await _contractService.Update(id, model);
            return Ok(new { Message = "Contract Updated Successfully", Data = result });
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
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContract(int id)
    {
        try
        {
            await _contractService.Remove(id);
            return Ok("Contract Removed Successfully");
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

    [Authorize]
    [HttpGet("services")]
    public async Task<IActionResult> GetServiceOfContract(int contractId)
    {
        try
        {
            var result = await _serviceContractService.Get(contractId);
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

    [Authorize]
    [HttpGet("services/select")]
    public async Task<IActionResult> GetSeletionList(int contractId)
    {
        try
        {
            var result = await _serviceContractService.GetServicesList(contractId);
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
    [HttpPost("services")]
    public async Task<IActionResult> AddServicesToContract(int contractId, List<int> serviceIds)
    {
        try
        {
            var result = await _serviceContractService.Add(contractId, serviceIds);
            return Ok(new { Message = "Services Added To Contract Successfully", Data = result });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.ACCOUNTANT}")]
    [HttpDelete("services/{id}")]
    public async Task<IActionResult> RemoveServiceOfContract(int id)
    {
        try
        {
            await _serviceContractService.Remove(id);
            return Ok("Services Removed Successfully");
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
