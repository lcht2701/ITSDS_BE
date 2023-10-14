using API.DTOs.Requests.Categories;
using API.DTOs.Requests.TicketSolutions;
using Domain.Constants;
using Domain.Constants.Enums;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Tickets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Helpers;
using Persistence.Repositories.Interfaces;

namespace API.Controllers
{
    [Route("/v1/itsds/solution")]
    public class TicketSolutionController : BaseController
    {
        private readonly IRepositoryBase<TicketSolution> _solutionRepository;
        private readonly IRepositoryBase<User> _userRepository;

        public TicketSolutionController(IRepositoryBase<TicketSolution> solutionRepository, IRepositoryBase<User> userRepository)
        {
            _solutionRepository = solutionRepository;
            _userRepository = userRepository;
        }

        [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN},{Roles.CUSTOMER}")]
        [HttpGet]
        public async Task<IActionResult> GetAllSolutions()
        {
            var user = await _userRepository.FirstOrDefaultAsync(x => x.Id.Equals(CurrentUserID));
            List<TicketSolution> result;
            if (user.Role == Role.Customer)
            {
                result = (List<TicketSolution>)await _solutionRepository.WhereAsync(
                    x => x.IsPublic == true &&
                    x.IsApproved == true &&
                    x.ExpiredDate <= DateTime.UtcNow,
                    new string[] { "Category", "Owner" });
            }
            else
            {
                result = (List<TicketSolution>)await _solutionRepository.GetAsync(navigationProperties: new string[] { "Category", "Owner" });
            }

            var response = result.Select(solution =>
            {
                var entity = Mapper.Map(solution, new TicketSolution());
                entity.ReviewDate = DataResponse.CleanNullableDateTime(solution.ReviewDate);
                entity.ExpiredDate = DataResponse.CleanNullableDateTime(solution.ExpiredDate);

                return entity;
            }).ToList();

            return response != null ? Ok(response) : Ok();
        }

        [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN},{Roles.CUSTOMER}")]
        [HttpGet("{solutionId}")]
        public async Task<IActionResult> GetSolutionById(int solutionId)
        {
            var result = await _solutionRepository.FirstOrDefaultAsync(x => x.Id.Equals(solutionId),
                new string[] { "Category", "Owner" });
            return result != null ? Ok(result) : throw new BadRequestException("Solution not found");
        }

        [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN}")]
        [HttpPost("new")]
        public async Task<IActionResult> CreateSolution([FromBody] CreateTicketSolutionRequest model)
        {
            var entity = Mapper.Map(model, new TicketSolution());
            await _solutionRepository.CreateAsync(entity);
            return Ok("Create Successfully");
        }

        [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN}")]
        [HttpPut("{solutionId}")]
        public async Task<IActionResult> UpdateSolution(int solutionId, [FromBody] UpdateTicketSolutionRequest req)
        {
            var target = await _solutionRepository.FoundOrThrow(c => c.Id.Equals(solutionId), new BadRequestException("Solution not found"));
            TicketSolution entity = Mapper.Map(req, target);
            await _solutionRepository.UpdateAsync(entity);
            return Accepted("Update Successfully");
        }

        [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN}")]
        [HttpDelete("{categoryId}")]
        public async Task<IActionResult> DeleteSolution(int solutionId)
        {
            var target = await _solutionRepository.FoundOrThrow(c => c.Id.Equals(solutionId), new BadRequestException("Solution not found"));
            //Soft Delete
            await _solutionRepository.DeleteAsync(target);
            return Ok("Delete Successfully");
        }

        [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN}")]
        [HttpDelete("change-public")]
        public async Task<IActionResult> ChangePublic(int solutionId)
        {
            var target = await _solutionRepository.FoundOrThrow(c => c.Id.Equals(solutionId), new BadRequestException("Solution not found"));
            target.IsPublic = !target.IsPublic;
            await _solutionRepository.UpdateAsync(target);
            return Ok("Update Successfully");
        }

        [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN}")]
        [HttpDelete("approve")]
        public async Task<IActionResult> ApproveSolution(int solutionId)
        {
            var target = await _solutionRepository.FoundOrThrow(c => c.Id.Equals(solutionId), new BadRequestException("Solution not found"));
            target.IsApproved = !target.IsApproved;
            await _solutionRepository.UpdateAsync(target);
            return Ok("Approve Successfully");
        }
    }
}
