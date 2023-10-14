﻿using API.DTOs.Requests.Categories;
using API.DTOs.Requests.Contracts;
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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllCategory()
        {
            var result = await _categoryRepository.ToListAsync();
            var sortedList = result.OrderBy(x => x.Name);
            return result != null ? Ok(sortedList) : Ok("No Categories");
        }

        [Authorize(Roles = Roles.MANAGER)]
        [HttpGet("{categoryId}")]
        public async Task<IActionResult> GetCategoryById(int categoryId)
        {
            var result = await _categoryRepository.FirstOrDefaultAsync(x => x.Id.Equals(categoryId), new string[] { "AssignedTechnical" });
            return result != null ? Ok(result) : throw new BadRequestException("Category not found.");
        }

        [Authorize(Roles = Roles.MANAGER)]
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoriesRequest model)
        {
            if (model.AssignedTechnicalId != null)
            {
                User user = await _userrepository.FoundOrThrow(x => x.Id.Equals(model.AssignedTechnicalId), new BadRequestException("User not found"));
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
                User user = await _userrepository.FoundOrThrow(x => x.Id.Equals(req.AssignedTechnicalId), new BadRequestException("User not found"));
                if (user.Role != Role.Technician)
                {
                    throw new BadRequestException("Cannot assign this user.");
                }
            }
            var target = await _categoryRepository.FoundOrThrow(c => c.Id.Equals(categoryId), new BadRequestException("Category not found"));
            Category entity = Mapper.Map(req, target);
            await _categoryRepository.UpdateAsync(entity);
            return Accepted("Update Successfully");
        }

        [Authorize(Roles = Roles.MANAGER)]
        [HttpDelete("{categoryId}")]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var target = await _categoryRepository.FoundOrThrow(c => c.Id.Equals(categoryId), new BadRequestException("Category not found"));
            //Soft Delete
            await _categoryRepository.DeleteAsync(target);
            return Ok("Delete Successfully");
        }
    }
}
