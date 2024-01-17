using API.DTOs.Requests.Categories;
using API.DTOs.Responses.Tickets;
using API.Services.Interfaces;
using Domain.Constants;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Helpers;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    [Route("/v1/itsds/category")]
    public class CategoryController : BaseController
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [Authorize]
        [HttpGet("all")]

        public async Task<IActionResult> GetAllCategory()
        {
            var result = await _categoryService.Get();
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        [SwaggerResponse(200, "Get List Categories", typeof(List<Domain.Models.Tickets.Category>))]
        public async Task<IActionResult> GetCategories(
        [FromQuery] string? filter,
        [FromQuery] string? sort,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5)
        {
            var result = await _categoryService.Get();
            var pagedResponse = result.AsQueryable().GetPagedData(page, pageSize, filter, sort);
            int totalPage = (int)Math.Ceiling((double)result.Count / pageSize);
            return Ok(new { TotalPage = totalPage, Data = pagedResponse });
        }

        [Authorize(Roles = Roles.MANAGER)]
        [HttpGet("{categoryId}")]
        [SwaggerResponse(200, "Get Categories By Id", typeof(List<Domain.Models.Tickets.Category>))]
        public async Task<IActionResult> GetCategoryById(int categoryId)
        {
            try
            {
                var result = await _categoryService.GetById(categoryId);
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

        [Authorize(Roles = Roles.MANAGER)]
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoriesRequest model)
        {
            try
            {
                var entity = await _categoryService.Create(model);
                return Ok(new
                {
                    Message = "Category Created Successfully",
                    Data = entity
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
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
        [HttpPut("{categoryId}")]
        public async Task<IActionResult> UpdateCategory(int categoryId, [FromBody] UpdateCategoriesRequest model)
        {
            try
            {
                var entity = await _categoryService.Update(categoryId, model);
                return Ok(new
                {
                    Message = "Category Updated Successfully",
                    Data = entity
                });
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
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

        [Authorize(Roles = Roles.MANAGER)]
        [HttpDelete("{categoryId}")]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            try
            {
                await _categoryService.Remove(categoryId);
                return Ok("Category Deleted Successfully");
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
    }
}
