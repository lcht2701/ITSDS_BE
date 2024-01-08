using API.DTOs.Requests.CompanyAddresss;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Models;
using Domain.Models.Contracts;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements
{
    public class CompanyAddressService : ICompanyAddressService
    {
        private readonly IRepositoryBase<CompanyAddress> _companyAddressRepository;
        private readonly IRepositoryBase<User> _userRepository;
        private readonly IRepositoryBase<CompanyMember> _companyMemberRepository;
        private readonly IMapper _mapper;

        public CompanyAddressService(IRepositoryBase<CompanyAddress> companyAddressRepository, IRepositoryBase<User> userRepository, IRepositoryBase<CompanyMember> companyMemberRepository, IMapper mapper)
        {
            _companyAddressRepository = companyAddressRepository;
            _userRepository = userRepository;
            _companyMemberRepository = companyMemberRepository;
            _mapper = mapper;
        }

        public async Task<List<CompanyAddress>> Get(int companyId)
        {
            var result = (await _companyAddressRepository.WhereAsync(x => x.CompanyId.Equals(companyId))).ToList();
            return result;
        }

        public async Task<CompanyAddress> GetById(int id)
        {
            var result = await _companyAddressRepository.FirstOrDefaultAsync(x => x.Id.Equals(id)) ?? throw new KeyNotFoundException("CompanyAddress is not found");
            return result;
        }

        public async Task<CompanyAddress> Create(int companyId, CreateCompanyAddressRequest model)
        {
            var newCompanyAddress = _mapper.Map(model, new CompanyAddress());
            newCompanyAddress.CompanyId = companyId;
            var result = await _companyAddressRepository.CreateAsync(newCompanyAddress);
            return result;
        }

        public async Task<CompanyAddress> Update(int id, UpdateCompanyAddressRequest model)
        {
            var target = await _companyAddressRepository.FirstOrDefaultAsync(x => x.Id.Equals(id)) ?? throw new KeyNotFoundException("CompanyAddress is not found");
            var entity = _mapper.Map(model, target);
            var result = await _companyAddressRepository.UpdateAsync(entity);
            return result;
        }

        public async Task Remove(int id)
        {
            var target = await _companyAddressRepository.FirstOrDefaultAsync(x => x.Id.Equals(id)) ?? throw new KeyNotFoundException("CompanyAddress is not found");
            await _companyAddressRepository.DeleteAsync(target);
        }

        public async Task RemoveByCompany(int companyId)
        {
            var target = await _companyAddressRepository.WhereAsync(x => x.CompanyId.Equals(companyId));
            foreach (var entity in target)
            {
                await _companyAddressRepository.DeleteAsync(entity);
            }
        }
    }
}
