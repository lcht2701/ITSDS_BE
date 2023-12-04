
using API.DTOs.Requests.Contracts;
using API.DTOs.Requests.ServiceContracts;
using API.Services.Interfaces;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Helpers;

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

    [Authorize]
    [HttpGet("parent-contracts")]

    public async Task<IActionResult> GetParentContracts()
    {
        var result = await _contractService.GetParentContracts();
        return Ok(result);
    }

    [Authorize]
    [HttpGet("child")]
    public async Task<IActionResult> GetChildContracts(int contractId)
    {
        try
        {
            var result = await _contractService.GetChildContracts(contractId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpGet("renew")]
    public async Task<IActionResult> GetRenewals(int contractId)
    {
        try
        {
            var result = await _contractService.GetContractRenewals(contractId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.ACCOUNTANT}")]
    [HttpGet("accountant")]
    public async Task<IActionResult> GetByAccountant()
    {
        try
        {
            var result = await _contractService.GetByAccountant(CurrentUserID);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpGet("customer")]
    public async Task<IActionResult> GetByCustomer()
    {
        try
        {
            var result = await _contractService.GetByCustomer(CurrentUserID);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetContractById(int id)
    {
        try
        {
            var result = await _contractService.GetById(id);
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

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.ACCOUNTANT}")]
    [HttpPut("{contractId}/renew")]
    public async Task<IActionResult> RenewContract(int contractId, [FromBody] RenewContractRequest model)
    {
        try
        {
            var result = await _contractService.RenewContract(contractId, model, CurrentUserID);
            return Ok(new { Message = "Contract Renewed Successfully", Data = result });
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

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.ACCOUNTANT}")]
    [HttpPost("generate-periodic-tickets")]
    public async Task<IActionResult> CreatePeriodicTickets(int id)
    {
        try
        {
            var result = await _serviceContractService.CreatePeriodicTickets(id, CurrentUserID);
            return Ok(new { Message = "Periodic Tickets Created Successfully", Data = result });
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
