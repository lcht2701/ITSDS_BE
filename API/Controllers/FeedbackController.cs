using API.DTOs.Requests.Feedbacks;
using API.Services.Interfaces;
using Domain.Constants;
using Domain.Constants.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Helpers;
using Persistence.Repositories.Interfaces;

namespace API.Controllers;

[Route("/v1/itsds/solution/feedback")]
public class FeedbackController : BaseController
{
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IFeedbackService _feedbackService;

    public FeedbackController(IRepositoryBase<User> userRepository, IFeedbackService feedbackService)
    {
        _userRepository = userRepository;
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
            var user = await _userRepository.FirstOrDefaultAsync(x => x.Id.Equals(CurrentUserID));
            switch (user.Role)
            {
                case Role.Manager:
                case Role.Technician:
                    var result1 = await _feedbackService.Get(solutionId);
                    var pagedResponse1 = result1.AsQueryable().GetPagedData(page, pageSize, filter, sort);
                    return Ok(pagedResponse1);
                case Role.Customer:
                    var result2 = await _feedbackService.GetByCustomer(solutionId);
                    var pagedResponse2 = result2.AsQueryable().GetPagedData(page, pageSize, filter, sort);
                    return Ok(pagedResponse2);
                default: return BadRequest();
            }
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Solution is not exist");
        }
    }

    [Authorize(Roles = $"{Roles.CUSTOMER},{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpPost]
    public async Task<IActionResult> CreateFeedback(int solutionId, [FromBody] CreateFeedbackRequest model)
    {
        try
        {
            await _feedbackService.Create(solutionId, model, CurrentUserID);
            return Ok("Created Successfully");
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
            await _feedbackService.Update(feedbackId, model, CurrentUserID);
            return Ok("Updated Successfully");
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

    [Authorize(Roles = $"{Roles.CUSTOMER},{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpDelete("{feedbackId}")]
    public async Task<IActionResult> DeleteFeedback(int feedbackId)
    {
        try
        {
            await _feedbackService.Delete(feedbackId);
            return Ok("Deleted Successfully");
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
