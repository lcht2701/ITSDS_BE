using API.DTOs.Requests.Categories;
using Domain.Models.Tickets;

namespace API.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<Category>> Get();
        Task<Category> GetById(int id);
        Task<Category> Create(CreateCategoriesRequest model);
        Task<Category> Update(int id, UpdateCategoriesRequest model);
        Task Remove(int id);
    }
}
