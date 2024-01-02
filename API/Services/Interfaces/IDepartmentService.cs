using API.DTOs.Requests.Departments;
using Domain.Models.Contracts;

namespace API.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<List<Department>> GetByCompany(int companyId);
        Task<Department> GetById(int id);
        Task<Department> Create(int companyId, CreateDepartmentRequest model);
        Task<Department> Update(int id, UpdateDepartmentRequest model);
        Task Remove(int id);
    }
}
