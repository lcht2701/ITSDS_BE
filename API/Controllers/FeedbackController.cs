using API.DTOs.Requests.Feedbacks;
using API.Services.Interfaces;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Helpers;

namespace API.Controllers;

[Route("/v1/itsds/solution/feedback")]
public class FeedbackController : BaseController
{
    private readonly IFeedbackService _feedbackService;

    public FeedbackController(IFeedbackService feedbackService)
    {
        _feedbackService = feedbackService;
    }

    [Authorize(Roles = $"{Roles.CUSTOMER},{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpGet]
    public async Task<IActionResult> GetFeedbacks(
        int solutionId,
        [FromQuery] string? filter,
        [FromQuery] string? sort,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5)
    {
        try
        {
            var result = await _feedbackService.Get(solutionId, CurrentUserID);
            var pagedResponse = result.AsQueryable().GetPagedData(page, pageSize, filter, sort);
            return Ok(pagedResponse);
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

    [Authorize(Roles = $"{Roles.CUSTOMER},{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var result = await _feedbackService.GetById(id);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.CUSTOMER},{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpPost]
    public async Task<IActionResult> CreateFeedback([FromBody] CreateFeedbackRequest model)
    {
        try
        {
            var result = await _feedbackService.Create(model, CurrentUserID);
            return Ok(new { Message = "Feedback Added Successfully", Data = result });
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.CUSTOMER},{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpPost("reply")]
    public async Task<IActionResult> CreateReply([FromBody] CreateReplyRequest model)
    {
        try
        {
            var result = await _feedbackService.CreateReply(model, CurrentUserID);
            return Ok(new { Message = "Reply Added Successfully", Data = result });
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.CUSTOMER},{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpPut("{feedbackId}")]
    public async Task<IActionResult> UpdateFeedback(int feedbackId, [FromBody] UpdateFeedbackRequest model)
    {
        try
        {
            var result = await _feedbackService.Update(feedbackId, model, CurrentUserID);
            return Ok(new { Message = "Feedback Updated Successfully", Data = result });
        }
        catch (KeyNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = $"{Roles.CUSTOMER},{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpDelete("{feedbackId}")]
    public async Task<IActionResult> DeleteFeedback(int feedbackId)
    {
        try
        {
            await _feedbackService.Delete(feedbackId);
            return Ok("Feedback Deleted Successfully");
        }
        catch (KeyNotFoundException)
        {
            return BadRequest("Feedback is not exist");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
