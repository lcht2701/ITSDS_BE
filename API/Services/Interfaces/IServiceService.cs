using API.DTOs.Requests.Services;
using Domain.Models.Contracts;

namespace API.Services.Interfaces;

public interface IServiceService
{
    Task<List<Service>> Get();
    Task<List<Service>> GetByCategory(int categoryId);
    Task<Service> GetById(int id);
    Task<Service> Create(CreateServiceRequest model);
    Task<Service> Update(int id, UpdateServiceRequest model);
    Task Remove(int id);
}
