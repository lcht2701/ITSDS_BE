using API.DTOs.Requests.Companies;
using API.DTOs.Responses.Companies;
using Domain.Models.Contracts;

namespace API.Services.Interfaces;

public interface ICompanyService
{
    Task<List<GetCompanyResponse>> Get();
    Task<GetCompanyResponse> GetById(int id);
    Task<Company> Create(CreateCompanyRequest model);
    Task<Company> Update(int id, UpdateCompanyRequest model);
    Task Remove(int id);
}
