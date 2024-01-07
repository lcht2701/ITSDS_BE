using API.DTOs.Requests.CompanyAddresss;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Constants.Enums;
using Domain.Models;
using Domain.Models.Contracts;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements
{
    public class CompanyAddressService : ICompanyAddressService
    {
        private readonly IRepositoryBase<CompanyAddress> _CompanyAddressRepository;
        private readonly IRepositoryBase<User> _userRepository;
        private readonly IRepositoryBase<CompanyMember> _companyMemberRepository;
        private readonly IMapper _mapper;

        public CompanyAddressService(IRepositoryBase<CompanyAddress> CompanyAddressRepository, IRepositoryBase<User> userRepository, IRepositoryBase<CompanyMember> companyMemberRepository, IMapper mapper)
        {
            _CompanyAddressRepository = CompanyAddressRepository;
            _userRepository = userRepository;
            _companyMemberRepository = companyMemberRepository;
            _mapper = mapper;
        }

        public async Task<List<CompanyAddress>> Get(int companyId)
        {
            var result = (await _CompanyAddressRepository.WhereAsync(x => x.CompanyId.Equals(companyId))).ToList();
            return result;
        }

        public async Task<CompanyAddress> GetById(int id)
        {
            var result = await _CompanyAddressRepository.FirstOrDefaultAsync(x => x.Id.Equals(id)) ?? throw new KeyNotFoundException("CompanyAddress is not found");
            return result;
        }

        public async Task<CompanyAddress> Create(int companyId, CreateCompanyAddressRequest model)
        {
            var newCompanyAddress = _mapper.Map(model, new CompanyAddress());
            newCompanyAddress.CompanyId = companyId;
            var result = await _CompanyAddressRepository.CreateAsync(newCompanyAddress);
            return result;
        }

        public async Task<CompanyAddress> Update(int id, UpdateCompanyAddressRequest model)
        {
            var target = await _CompanyAddressRepository.FirstOrDefaultAsync(x => x.Id.Equals(id)) ?? throw new KeyNotFoundException("CompanyAddress is not found");
            var entity = _mapper.Map(model, target);
            var result = await _CompanyAddressRepository.UpdateAsync(entity);
            return result;
        }

        public async Task Remove(int id)
        {
            var target = await _CompanyAddressRepository.FirstOrDefaultAsync(x => x.Id.Equals(id)) ?? throw new KeyNotFoundException("CompanyAddress is not found");
            await _CompanyAddressRepository.SoftDeleteAsync(target);
        }
    }
}
