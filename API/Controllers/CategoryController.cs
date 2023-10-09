using API.DTOs.Requests.Categories;
using API.DTOs.Requests.Contracts;
using API.DTOs.Requests.Teams;
using Domain.Constants;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Tickets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Repositories.Interfaces;
using System.Diagnostics.Contracts;

namespace API.Controllers
{
    [Route("/v1/itsds/category")]
    public class CategoryController : BaseController
    {
        private readonly IRepositoryBase<Category> _categoryRepository;
        private readonly IRepositoryBase<User> _userrepository;

        public CategoryController(IRepositoryBase<Category> categoryRepository, IRepositoryBase<User> userrepository)
        {
            _categoryRepository = categoryRepository;
            _userrepository = userrepository;
        }

        [Authorize(Roles = $"{Roles.CUSTOMER},{Roles.MANAGER}")]
        [HttpGet]
        public async Task<IActionResult> GetAllCategory()
        {
            var result = await _categoryRepository.ToListAsync();
            return Ok(result);
        }

        [Authorize(Roles = Roles.MANAGER)]
        [HttpGet("{categoryId}")]
        public async Task<IActionResult> GetCategoryById(int categoryId)
        {
            var result = await _categoryRepository.FoundOrThrow(x => x.Id.Equals(categoryId), new NotFoundException("Category not found"));
            return Ok(result);
        }

        [Authorize(Roles = Roles.MANAGER)]
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoriesRequest model)
        {
            if (model.AssignedTechnicalId != null)
            {
                User user = await _userrepository.FoundOrThrow(x => x.Id.Equals(model.AssignedTechnicalId), new NotFoundException("User not found"));
                if (user.Role != Role.Technician)
                {
                    throw new BadRequestException("Cannot assign this user.");
                }
            }
            var entity = Mapper.Map(model, new Category());
            await _categoryRepository.CreateAsync(entity);
            return Ok();
        }

        [Authorize(Roles = Roles.MANAGER)]
        [HttpPut("{categoryId}")]
        public async Task<IActionResult> UpdateCategory(int categoryId, [FromBody] UpdateCategoriesRequest req)
        {
            if (req.AssignedTechnicalId != null)
            {
                User user = await _userrepository.FoundOrThrow(x => x.Id.Equals(req.AssignedTechnicalId), new NotFoundException("User not found"));
                if (user.Role != Role.Technician)
                {
                    throw new BadRequestException("Cannot assign this user.");
                }
            }
            var target = await _categoryRepository.FoundOrThrow(c => c.Id.Equals(categoryId), new NotFoundException("Category not found"));
            Category entity = Mapper.Map(req, target);
            await _categoryRepository.UpdateAsync(entity);
            return Accepted("Updated Successfully");
        }

        [Authorize(Roles = Roles.MANAGER)]
        [HttpDelete("{categoryId}")]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var target = await _categoryRepository.FoundOrThrow(c => c.Id.Equals(categoryId), new NotFoundException("Category not found"));
            //Soft Delete
            await _categoryRepository.DeleteAsync(target);
            return Ok("Deleted Successfully");
        }
    }
}
