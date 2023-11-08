using API.DTOs.Requests.Companies;
using Domain.Models.Contracts;

namespace API.Services.Interfaces;

public interface ICompanyService
{
    Task<List<Company>> Get();
    Task<Company> GetById(int id);
    Task<Company> Create(CreateCompanyRequest model);
    Task<Company> Update(int id, UpdateCompanyRequest model);
    Task Remove(int id);
}
