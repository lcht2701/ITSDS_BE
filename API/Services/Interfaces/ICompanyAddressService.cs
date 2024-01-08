using API.DTOs.Requests.CompanyAddresss;
using Domain.Models.Contracts;

namespace API.Services.Interfaces
{
    public interface ICompanyAddressService
    {
        Task<List<CompanyAddress>> Get(int companyId);
        Task<CompanyAddress> GetById(int id);
        Task<CompanyAddress> Create(int companyId, CreateCompanyAddressRequest model);
        Task<CompanyAddress> Update(int id, UpdateCompanyAddressRequest model);
        Task Remove(int id);
        Task RemoveByCompany(int companyId);
    }
}
