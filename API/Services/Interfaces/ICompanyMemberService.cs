using API.DTOs.Requests.CompanyMembers;
using Domain.Models.Contracts;

namespace API.Services.Interfaces
{
    public interface ICompanyMemberService
    {
        Task<List<CompanyMember>> Get(int companyId);
        Task<List<CompanyMember>> GetMemberNotInCompany(int companyId);
        Task<CompanyMember> GetById(int id);
        Task<CompanyMember> Create(AddCompanyMemberRequest model);
        Task<CompanyMember> Update(int id, UpdateCompanyMemberRequest model);
        Task Remove(int id);
    }
}
