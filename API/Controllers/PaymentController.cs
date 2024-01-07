using API.DTOs.Requests.Payments;
using API.DTOs.Requests.PaymentTerms;
using API.Services.Interfaces;
using Domain.Constants;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Helpers;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

[Route("/v1/itsds/payment")]
public class PaymentController : BaseController
{
    private readonly IPaymentService _paymentService;
    private readonly IPaymentTermService _termService;

    public PaymentController(IPaymentService paymentService, IPaymentTermService termService)
    {
        _paymentService = paymentService;
        _termService = termService;
    }

    [Authorize]
    [HttpGet("all")]

    public async Task<IActionResult> GetAll()
    {
        try
        {
            return Ok(await _paymentService.Get());
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.ACCOUNTANT}")]
    [HttpGet]
    [SwaggerResponse(200, "Get Payment", typeof(List<Domain.Models.Contracts.Payment>))]
    public async Task<IActionResult> Get(
    [FromQuery] string? filter,
    [FromQuery] string? sort,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 5)
    {
        try
        {
            var teams = await _paymentService.Get();
            var pagedResponse = teams.AsQueryable().GetPagedData(page, pageSize, filter, sort);
            return Ok(pagedResponse);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [Authorize(Roles = $"{Roles.MANAGER},{Roles.ACCOUNTANT}")]
    [HttpGet("contract/{contractId}")]
    [SwaggerResponse(200, "Get Payment By Contract", typeof(List<Domain.Models.Contracts.Payment>))]
    public async Task<IActionResult> GetByContract(int contractId)
    {
        try
        {
            var result = await _paymentService.GetByContract(contractId);
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
    [HttpGet("{id}")]
    [SwaggerResponse(200, "Get Payment By Id", typeof(Domain.Models.Contracts.Payment))]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var result = await _paymentService.GetById(id);
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
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest model)
    {
        try
        {
            var result = await _paymentService.Create(model);
            return Ok(new { Message = "Payment Created Successfully", Data = result });

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.ACCOUNTANT}")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePayment(int id, [FromBody] UpdatePaymentRequest model)
    {
        try
        {
            var result = await _paymentService.Update(id, model);
            return Ok(new { Message = "Payment Updated Successfully", Data = result });
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
    public async Task<IActionResult> DeletePayment(int id)
    {
        try
        {
            await _paymentService.Remove(id);
            return Ok("Payment Removed Successfully");
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

    //[Authorize(Roles = $"{Roles.MANAGER},{Roles.ACCOUNTANT}")]
    //[HttpPatch("{id}")]
    //public async Task<IActionResult> ClosePayment(int id)
    //{
    //    try
    //    {
    //        await _paymentService.ClosePayment(id);
    //        return Ok("Payment Closed Successfully");
    //    }
    //    catch (KeyNotFoundException ex)
    //    {
    //        return NotFound(ex.Message);
    //    }
    //    catch (BadRequestException ex)
    //    {
    //        return NotFound(ex.Message);
    //    }
    //    catch (Exception ex)
    //    {
    //        return BadRequest(ex.Message);
    //    }
    //}

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.ACCOUNTANT}")]
    [HttpGet("term")]
    public async Task<IActionResult> GetPaymentTerms(int paymentId)
    {
        try
        {
            var result = await _termService.GetPaymentTerms(paymentId);
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
    [HttpGet("term/{id}")]
    public async Task<IActionResult> GetPaymentTermById(int id)
    {
        try
        {
            var result = await _termService.GetPaymentTermById(id);
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
    [HttpPost("term/{paymentId}")]
    public async Task<IActionResult> CreateTerms(int paymentId)
    {
        try
        {
            var result = await _termService.GeneratePaymentTerms(paymentId);
            return Ok(new { Message = "Payment Terms Created Successfully", Data = result });

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.ACCOUNTANT}")]
    [HttpPut("term/{id}")]
    public async Task<IActionResult> UpdateTerm(int id, [FromBody] UpdatePaymentTermRequest model)
    {
        try
        {
            var result = await _termService.UpdatePaymentTerm(id, model);
            return Ok(new { Message = "Payment Term Updated Successfully", Data = result });
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
    [HttpDelete("term/{paymentId}")]
    public async Task<IActionResult> DeleteTerm(int paymentId)
    {
        try
        {
            await _termService.RemovePaymentTerm(paymentId);
            return Ok("Payment Terms Removed Successfully");
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
