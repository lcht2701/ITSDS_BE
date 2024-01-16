
using API.DTOs.Requests.Contracts;
using API.DTOs.Responses.Contracts;
using API.Services.Interfaces;
using Domain.Constants;
using Domain.Exceptions;
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
    private readonly IPaymentService _paymentService;

    public ContractController(IContractService contractService, IServiceContractService serviceContractService, IPaymentService paymentService)
    {
        _contractService = contractService;
        _serviceContractService = serviceContractService;
        _paymentService = paymentService;
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
        int totalPage = (int)Math.Ceiling((double)result.Count / pageSize);
        return Ok(new { TotalPage = totalPage, Data = pagedResponse });
    }

    [Authorize(Roles = Roles.CUSTOMER)]
    [HttpGet("company")]
    [SwaggerResponse(200, "Get Contract By Company Admin", typeof(List<GetContractResponse>))]
    public async Task<IActionResult> GetByCompanyAdmin(
    [FromQuery] string? filter,
    [FromQuery] string? sort,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 5)
    {
        var result = await _contractService.GetByCompanyAdmin(CurrentUserID);
        var pagedResponse = result.AsQueryable().GetPagedData(page, pageSize, filter, sort);
        int totalPage = (int)Math.Ceiling((double)result.Count / pageSize);
        return Ok(new { TotalPage = totalPage, Data = pagedResponse });

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
            var contract = await _contractService.Create(model);
            await _serviceContractService.AddAndUpdate(contract.Id, model.ServiceIds);
            return Ok(new { Message = "Contract Created Successfully", Data = contract });
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
            var contract = await _contractService.Update(id, model);
            await _serviceContractService.AddAndUpdate(contract.Id, model.ServiceIds);
            return Ok(new { Message = "Contract Updated Successfully", Data = contract });
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
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContract(int id)
    {
        try
        {
            var payment = await _paymentService.GetByContract(id);
            //Remove Payment of Contract
            await _paymentService.Remove(payment.Id);
            //Remove Services of Contract
            await _serviceContractService.Remove(id);
            //Remove Contract
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
    public async Task<IActionResult> GetServicesList(int contractId)
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
}
