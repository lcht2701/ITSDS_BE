using API.DTOs.Requests.Payments;
using API.DTOs.Requests.PaymentTerms;
using API.Services.Interfaces;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Helpers;

namespace API.Controllers;

[Route("/v1/itsds/payment")]
public class PaymentController : BaseController
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
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
    public async Task<IActionResult> GetByContract(int contractId)
    {
        try
        {
            var result = await _paymentService.GetByContract(contractId);
            return Ok(result);
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

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.ACCOUNTANT}")]
    [HttpPost("term/{paymentId}")]
    public async Task<IActionResult> CreateTerms(int paymentId)
    {
        try
        {
            var result = await _paymentService.GeneratePaymentTerms(paymentId);
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
            var result = await _paymentService.UpdatePaymentTerm(id, model);
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
    [HttpDelete("term/{id}")]
    public async Task<IActionResult> DeleteTerm(int id)
    {
        try
        {
            await _paymentService.RemovePaymentTerm(id);
            return Ok("Payment Term Removed Successfully");
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
    [HttpPost("term/send-notification")]
    public async Task<IActionResult> SendNotification(int id)
    {
        try
        {
            await _paymentService.SendPaymentNotification(id);
            return Ok("Payment Term Notification Sent Successfully");

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

}
