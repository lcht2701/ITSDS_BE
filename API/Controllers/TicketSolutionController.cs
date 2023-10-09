using API.DTOs.Requests.Categories;
using API.DTOs.Requests.TicketSolutions;
using Domain.Constants;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Tickets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            List<TicketSolution>? result;
            if (user.Role == Role.Customer)
            {
                result = (List<TicketSolution>?)await _solutionRepository.WhereAsync(
                    x => x.IsPublic == true && 
                    x.IsApproved == true && 
                    x.ExpiredDate <= DateTime.UtcNow);
            }
            else
            {
            result = await _solutionRepository.ToListAsync();
            }
            return Ok(result);
        }

        [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN}")]
        [HttpPost("new-solution")]
        public async Task<IActionResult> CreateSolution([FromBody] CreateTicketSolutionRequest model)
        { 
            var entity = Mapper.Map(model, new TicketSolution());
            await _solutionRepository.CreateAsync(entity);
            return Ok();
        }

        [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN}")]
        [HttpPut("{solutionId}")]
        public async Task<IActionResult> UpdateSolution(int solutionId, [FromBody] UpdateCategoriesRequest req)
        {
            var target = await _solutionRepository.FoundOrThrow(c => c.Id.Equals(solutionId), new NotFoundException("Solution not found"));
            TicketSolution entity = Mapper.Map(req, target);
            await _solutionRepository.UpdateAsync(entity);
            return Accepted("Updated Successfully");
        }

        [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN}")]
        [HttpDelete("{categoryId}")]
        public async Task<IActionResult> DeleteCategory(int solutionId)
        {
            var target = await _solutionRepository.FoundOrThrow(c => c.Id.Equals(solutionId), new NotFoundException("Solution not found"));
            //Soft Delete
            await _solutionRepository.DeleteAsync(target);
            return Ok("Deleted Successfully");
        }

        [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN}")]
        [HttpDelete("change-public")]
        public async Task<IActionResult> ChangePublic(int solutionId)
        {
            var target = await _solutionRepository.FoundOrThrow(c => c.Id.Equals(solutionId), new NotFoundException("Solution not found"));
            target.IsPublic = !target.IsPublic;
            await _solutionRepository.UpdateAsync(target);
            return Ok("Deleted Successfully");
        }

        [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN}")]
        [HttpDelete("approve")]
        public async Task<IActionResult> ApproveSolution(int solutionId)
        {
            var target = await _solutionRepository.FoundOrThrow(c => c.Id.Equals(solutionId), new NotFoundException("Solution not found"));
            target.IsApproved = !target.IsApproved;
            await _solutionRepository.UpdateAsync(target);
            return Ok("Deleted Successfully");
        }
    }
}
