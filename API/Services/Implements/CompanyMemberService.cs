using API.DTOs.Requests.CompanyMembers;
using API.Services.Interfaces;
using Domain.Models.Contracts;

namespace API.Services.Implements
{
    public class CompanyMemberService : ICompanyMemberService
    {
        public Task<CompanyMember> Create(AddCompanyMemberRequest model)
        {
            throw new NotImplementedException();
        }

        public Task<List<CompanyMember>> Get(int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<CompanyMember> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<CompanyMember>> GetMemberNotInCompany(int companyId)
        {
            throw new NotImplementedException();
        }

        public Task Remove(int id)
        {
            throw new NotImplementedException();
        }

        public Task<CompanyMember> Update(int id, UpdateCompanyMemberRequest model)
        {
            throw new NotImplementedException();
        }
    }
}
