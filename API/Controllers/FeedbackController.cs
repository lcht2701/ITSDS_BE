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
        [HttpGet]
        public async Task<IActionResult> GetFeedbacksOfSolution(int solutionId)
        {
            var user = await _userRepository.FirstOrDefaultAsync(x => x.Id.Equals(CurrentUserID), new string[] { "User", "TicketSolution" });
            IList<Feedback>? result;
            if (user.Role == Role.Customer)
            {
                result = await _feedbackRepository.WhereAsync(
                    x => x.SolutionId.Equals(solutionId) &&
                    x.IsPublic == true);
            }
            else
            {
                result = await _feedbackRepository.WhereAsync(x => x.SolutionId.Equals(solutionId), new string[] { "User", "TicketSolution" });
            }

            return result.Count != 0 ? Ok(result) : Ok("No Feedbacks");
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
            //Soft Delete
            await _feedbackRepository.DeleteAsync(target);
            return Ok("Delete Successfully");
        }
    }
}
