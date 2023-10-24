using API.DTOs.Requests.Categories;
using API.DTOs.Requests.Contracts;
using API.DTOs.Requests.Feedbacks;
using API.DTOs.Requests.Teams;
using Domain.Constants;
using Domain.Constants.Enums;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Tickets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Helpers;
using Persistence.Repositories.Interfaces;
using System.Diagnostics.Contracts;

namespace API.Controllers
{
    [Route("/v1/itsds/solution/feedback")]
    public class FeedbackController : BaseController
    {
        private readonly IRepositoryBase<Feedback> _feedbackRepository;
        private readonly IRepositoryBase<User> _userRepository;

        public FeedbackController(IRepositoryBase<Feedback> feedbackRepository, IRepositoryBase<User> userRepository)
        {
            _feedbackRepository = feedbackRepository;
            _userRepository = userRepository;
        }

        [Authorize]
        [HttpGet("/all")]

        public async Task<IActionResult> GetAllContract()
        {
            var result = await _feedbackRepository.ToListAsync();
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetFeedbacksOfSolution(
        [FromQuery] string? filter,
        [FromQuery] string? sort,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5)
        {
            var result = await _feedbackRepository.ToListAsync();
            var pagedResponse = result.AsQueryable().GetPagedData(page, pageSize, filter, sort);
            return Ok(pagedResponse);
        }

        [Authorize(Roles = $"{Roles.CUSTOMER},{Roles.MANAGER},{Roles.TECHNICIAN}")]
        [HttpGet("{feedbackId}")]
        public async Task<IActionResult> GetFeedbackById(int feedbackId)
        {
            var result = await _feedbackRepository.FirstOrDefaultAsync(x => x.Id.Equals(feedbackId), new string[] { "User", "TicketSolution" });
            return result != null ? Ok(result) : throw new BadRequestException("Feedback not found");
        }

        [Authorize(Roles = $"{Roles.CUSTOMER},{Roles.MANAGER},{Roles.TECHNICIAN}")]
        [HttpPost]
        public async Task<IActionResult> CreateFeedback(int solutionId, [FromBody] CreateFeedbackRequest model)
        {
            var user = await _userRepository.FoundOrThrow(x => x.Id.Equals(CurrentUserID), new BadRequestException("User not found"));
            var entity = Mapper.Map(model, new Feedback());
            entity.SolutionId = solutionId;
            entity.UserId = CurrentUserID;
            if (user.Role == Role.Customer)
            {
                entity.IsPublic = true;
            }
            await _feedbackRepository.CreateAsync(entity);
            return Ok();
        }

        [Authorize(Roles = $"{Roles.CUSTOMER},{Roles.MANAGER},{Roles.TECHNICIAN}")]
        [HttpPut("{feedbackId}")]
        public async Task<IActionResult> UpdateFeedback(int feedbackId, [FromBody] UpdateFeedbackRequest req)
        {
            var user = await _userRepository.FoundOrThrow(x => x.Id.Equals(CurrentUserID), new BadRequestException("User not found"));
            var target = await _feedbackRepository.FoundOrThrow(c => c.Id.Equals(feedbackId), new BadRequestException("Feedback not found"));
            Feedback entity = Mapper.Map(req, target);
            if (user.Role == Role.Customer)
            {
                entity.IsPublic = true;
            }
            await _feedbackRepository.UpdateAsync(entity);
            return Accepted("Update Successfully");
        }

        [Authorize(Roles = $"{Roles.CUSTOMER},{Roles.MANAGER},{Roles.TECHNICIAN}")]
        [HttpDelete("{feedbackId}")]
        public async Task<IActionResult> DeleteFeedback(int feedbackId)
        {
            var target = await _feedbackRepository.FoundOrThrow(c => c.Id.Equals(feedbackId), new BadRequestException("Feedback not found"));
            await _feedbackRepository.SoftDeleteAsync(target);
            return Ok("Delete Successfully");
        }
    }
}
