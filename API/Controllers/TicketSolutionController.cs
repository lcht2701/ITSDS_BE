using API.DTOs.Requests.TicketSolutions;
using API.Services.Interfaces;
using Domain.Constants;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Helpers;

namespace API.Controllers;

[Route("/v1/itsds/solution")]
public class TicketSolutionController : BaseController
{
    private readonly ITicketSolutionService _ticketSolutionService;
    private readonly IReactionService _reactionService;

    public TicketSolutionController(ITicketSolutionService ticketSolutionService, IReactionService reactionService)
    {
        _ticketSolutionService = ticketSolutionService;
        _reactionService = reactionService;
    }


    [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN},{Roles.CUSTOMER}")]
    [HttpGet]
    public async Task<IActionResult> GetSolutions(
        [FromQuery] string? filter,
        [FromQuery] string? sort,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5)
    {
        try
        {
            var result = await _ticketSolutionService.Get(CurrentUserID);
            var pagedResponse = result.AsQueryable().GetPagedData(page, pageSize, filter, sort);
            return Ok(pagedResponse);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpGet("unapproved")]
    public async Task<IActionResult> GetUnapprovedSolutions(
        [FromQuery] string? filter,
        [FromQuery] string? sort,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5)
    {
        try
        {
            var result = await _ticketSolutionService.GetUnapprovedSolutions(CurrentUserID);
            var pagedResponse = result.AsQueryable().GetPagedData(page, pageSize, filter, sort);
            return Ok(pagedResponse);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN},{Roles.CUSTOMER}")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllSolutions()
    {
        try
        {
            var result = await _ticketSolutionService.Get(CurrentUserID);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN},{Roles.CUSTOMER}")]
    [HttpGet("{solutionId}")]
    public async Task<IActionResult> GetSolutionById(int solutionId)
    {
        try
        {
            var result = await _ticketSolutionService.GetById(solutionId, CurrentUserID);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Solution is not exist");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpPost("new")]
    public async Task<IActionResult> CreateSolution([FromBody] CreateTicketSolutionRequest model)
    {
        try
        {
            await _ticketSolutionService.Create(model, CurrentUserID);
            return Ok("Created Successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpPut("{solutionId}")]
    public async Task<IActionResult> UpdateSolution(int solutionId, [FromBody] UpdateTicketSolutionRequest req)
    {
        try
        {
            await _ticketSolutionService.Update(solutionId, req, CurrentUserID);
            return Ok("Updated Successfully");
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Solution is not exist");
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

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpDelete("{solutionId}")]
    public async Task<IActionResult> DeleteSolution(int solutionId)
    {
        try
        {
            await _ticketSolutionService.Remove(solutionId, CurrentUserID);
            return Ok("Deleted Successfully");
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Solution is not exist");
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

    [Authorize(Roles = Roles.TECHNICIAN)]
    [HttpPatch("submit-approval")]
    public async Task<IActionResult> SubmitForApproval(int solutionId, [FromBody] SubmitApprovalRequest model)
    {
        try
        {
            await _ticketSolutionService.SubmitForApproval(solutionId, CurrentUserID, model);
            return Ok("Approval Request Sent Successfully");
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Solution is not exist");
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

    [Authorize(Roles = Roles.MANAGER)]
    [HttpPatch("approve")]
    public async Task<IActionResult> ApproveSolution(int solutionId, [FromBody] ApproveSolutionRequest model)
    {
        try
        {
            await _ticketSolutionService.Approve(solutionId, model);
            return Ok("Update Successfully");
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Solution is not exist");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = Roles.MANAGER)]
    [HttpPatch("reject")]
    public async Task<IActionResult> RejectSolution(int solutionId)
    {
        try
        {
            await _ticketSolutionService.Reject(solutionId);
            return Ok("Update Successfully");
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Solution is not exist");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [Authorize]
    [HttpPost("{solutionId}/like")]
    public async Task<IActionResult> Like(int solutionId)
    {
        try
        {
            string response = await _reactionService.Like(solutionId, CurrentUserID);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [Authorize]
    [HttpPost("{solutionId}/dislike")]
    public async Task<IActionResult> Dislike(int solutionId)
    {
        try
        {
            string response = await _reactionService.Dislike(solutionId, CurrentUserID);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
