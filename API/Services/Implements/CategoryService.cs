using API.DTOs.Requests.Categories;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Models.Tickets;
using Persistence.Helpers.Caching;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class CategoryService : ICategoryService
{
    private readonly IRepositoryBase<Category> _categoryRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;

    public CategoryService(IRepositoryBase<Category> categoryRepository, ICacheService cacheService, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _cacheService = cacheService;
        _mapper = mapper;
    }

    public async Task<List<Category>> Get()
    {
        var cacheData = _cacheService.GetData<List<Category>>("categories");
        if (cacheData == null || !cacheData.Any())
        {
            cacheData = await _categoryRepository.ToListAsync();
            var expiryTime = DateTimeOffset.Now.AddSeconds(30);
            _cacheService.SetData("categories", cacheData, expiryTime);
        }
        return cacheData;
    }

    public async Task<Category> GetById(int id)
    {
        var cacheData = _cacheService.GetData<Category>($"category-{id}");
        if (cacheData == null)
        {
            cacheData = await _categoryRepository.FirstOrDefaultAsync(x => x.Id.Equals(id)) ?? throw new KeyNotFoundException("Category is not exist");
            var expiryTime = DateTimeOffset.Now.AddSeconds(30);
            _cacheService.SetData($"category-{cacheData.Id}", cacheData, expiryTime);
        }
        return cacheData;
    }

    public async Task<Category> Create(CreateCategoriesRequest model)
    {
        var entity = _mapper.Map(model, new Category());
        var result = await _categoryRepository.CreateAsync(entity);
        #region Cache
        var expiryTime = DateTimeOffset.Now.AddSeconds(30);
        _cacheService.SetData($"category-{result.Id}", result, expiryTime);
        var cacheList = await _categoryRepository.ToListAsync();
        _cacheService.SetData("categories", cacheList, expiryTime);
        #endregion
        return result;
    }

    public async Task<Category> Update(int id, UpdateCategoriesRequest model)
    {
        var target = await _categoryRepository.FoundOrThrow(c => c.Id.Equals(id), new KeyNotFoundException("Category is not exist"));
        Category entity = _mapper.Map(model, target);
        var result = await _categoryRepository.UpdateAsync(entity);
        #region Cache
        var expiryTime = DateTimeOffset.Now.AddSeconds(30);
        _cacheService.SetData($"category-{result.Id}", result, expiryTime);
        var cacheList = await _categoryRepository.ToListAsync();
        _cacheService.SetData("categories", cacheList, expiryTime);
        #endregion
        return result;
    }

    public async Task Remove(int id)
    {
        var target = await _categoryRepository.FoundOrThrow(c => c.Id.Equals(id), new KeyNotFoundException("Category is not exist"));
        await _categoryRepository.SoftDeleteAsync(target);
        #region Cache
        var expiryTime = DateTimeOffset.Now.AddSeconds(30);
        _cacheService.RemoveData($"category-{target.Id}"); 
        var cacheList = await _categoryRepository.ToListAsync();
        _cacheService.SetData("categories", cacheList, expiryTime);
        #endregion
    }
}
