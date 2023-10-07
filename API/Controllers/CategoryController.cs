using API.DTOs.Requests.Categories;
using API.DTOs.Requests.Contracts;
using API.DTOs.Requests.Teams;
using Domain.Constants;
using Domain.Exceptions;
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
        private readonly IRepositoryBase<Category> _categoryrepository;

        public CategoryController(IRepositoryBase<Category> categoryrepository)
        {
            _categoryrepository = categoryrepository;
        }

        [Authorize(Roles = Roles.MANAGER)]
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var result = await _categoryrepository.ToListAsync();
            return Ok(result);
        }

        [Authorize(Roles = Roles.MANAGER)]
        [HttpGet("{categoryId}")]
        public async Task<IActionResult> GetCategoryById(int categoryId)
        {
            var result = await _categoryrepository.FoundOrThrow(x => x.Id.Equals(categoryId), new NotFoundException("Category not found"));
            return Ok(result);
        }

        [Authorize(Roles = Roles.MANAGER)]
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoriesRequest model)
        {
            var entity = Mapper.Map(model, new Category());
            await _categoryrepository.CreateAsync(entity);
            return Ok();
        }

        [Authorize(Roles = Roles.MANAGER)]
        [HttpPut("{categoryId}")]
        public async Task<IActionResult> UpdateCategory(int categoryId, [FromBody] UpdateCategoriesRequest req)
        {
            var target = await _categoryrepository.FoundOrThrow(c => c.Id.Equals(categoryId), new NotFoundException("Category not found"));
            Category entity = Mapper.Map(req, target);
            await _categoryrepository.UpdateAsync(entity);
            return Accepted("Updated Successfully");
        }

        [Authorize(Roles = Roles.MANAGER)]
        [HttpDelete("{categoryId}")]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var target = await _categoryrepository.FoundOrThrow(c => c.Id.Equals(categoryId), new NotFoundException("Category not found"));
            //Soft Delete
            await _categoryrepository.DeleteAsync(target);
            return Ok("Deleted Successfully");
        }
    }
}
