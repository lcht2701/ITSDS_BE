using API.DTOs.Requests.Categories;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Constants.Enums;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Tickets;
using Persistence.Helpers.Caching;
using Persistence.Repositories.Interfaces;
using System.Collections.Generic;

namespace API.Services.Implements;

public class CategoryService : ICategoryService
{
    private readonly IRepositoryBase<Category> _categoryRepository;
    private readonly IRepositoryBase<User> _userrepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;

    public CategoryService(IRepositoryBase<Category> categoryRepository, IRepositoryBase<User> userrepository, ICacheService cacheService, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _userrepository = userrepository;
        _cacheService = cacheService;
        _mapper = mapper;
    }

    public async Task<List<Category>> Get()
    {
        return await _cacheService.GetAsync(
            "categories",
            async() => await _categoryRepository.ToListAsync());
    }

    public async Task<Category> GetById(int id)
    {
        var result = await _categoryRepository.FirstOrDefaultAsync(x => x.Id.Equals(id), new string[] { "AssignedTechnical" }) ?? throw new KeyNotFoundException("Category is not exist");
        return result;
    }

    public async Task<Category> Create(CreateCategoriesRequest model)
    {
        if (model.AssignedTechnicalId != null)
        {
            User user = await _userrepository.FoundOrThrow(x => x.Id.Equals(model.AssignedTechnicalId), new KeyNotFoundException("Assigned Technical is not exist"));
            if (user.Role != Role.Technician)
            {
                throw new BadRequestException("Cannot assign non-technician user.");
            }
        }
        var entity = _mapper.Map(model, new Category());
        await _categoryRepository.CreateAsync(entity);
        return entity;
    }

    public async Task<Category> Update(int id, UpdateCategoriesRequest model)
    {
        if (model.AssignedTechnicalId != null)
        {
            User user = await _userrepository.FoundOrThrow(x => x.Id.Equals(model.AssignedTechnicalId), new KeyNotFoundException("Assigned Technical is not exist"));
            if (user.Role != Role.Technician)
            {
                throw new BadRequestException("Cannot assign this user.");
            }
        }
        var target = await _categoryRepository.FoundOrThrow(c => c.Id.Equals(id), new KeyNotFoundException("Category is not exist"));
        Category entity = _mapper.Map(model, target);
        await _categoryRepository.UpdateAsync(entity);
        return entity;
    }

    public async Task Remove(int id)
    {
        var target = await _categoryRepository.FoundOrThrow(c => c.Id.Equals(id), new KeyNotFoundException("Category is not exist"));
        await _categoryRepository.SoftDeleteAsync(target);
    }
}
