using API.DTOs.Requests.Departments;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Constants.Enums;
using Domain.Models;
using Domain.Models.Contracts;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IRepositoryBase<Department> _departmentRepository;
        private readonly IRepositoryBase<User> _userRepository;
        private readonly IRepositoryBase<CompanyMember> _companyMemberRepository;
        private readonly IMapper _mapper;

        public DepartmentService(IRepositoryBase<Department> departmentRepository, IRepositoryBase<User> userRepository, IRepositoryBase<CompanyMember> companyMemberRepository, IMapper mapper)
        {
            _departmentRepository = departmentRepository;
            _userRepository = userRepository;
            _companyMemberRepository = companyMemberRepository;
            _mapper = mapper;
        }

        public async Task<List<Department>> Get(int companyId)
        {
            var result = (await _departmentRepository.WhereAsync(x => x.CompanyId.Equals(companyId))).ToList();
            return result;
        }

        public async Task<Department> GetById(int id)
        {
            var result = await _departmentRepository.FirstOrDefaultAsync(x => x.Id.Equals(id)) ?? throw new KeyNotFoundException("Department is not found");
            return result;
        }

        public async Task<Department> Create(int companyId, CreateDepartmentRequest model)
        {
            var newDepartment = _mapper.Map(model, new Department());
            newDepartment.CompanyId = companyId;
            var result = await _departmentRepository.CreateAsync(newDepartment);
            return result;
        }

        public async Task<Department> Update(int id, UpdateDepartmentRequest model)
        {
            var target = await _departmentRepository.FirstOrDefaultAsync(x => x.Id.Equals(id)) ?? throw new KeyNotFoundException("Department is not found");
            var entity = _mapper.Map(model, target);
            var result = await _departmentRepository.UpdateAsync(entity);
            return result;
        }

        public async Task Remove(int id)
        {
            var target = await _departmentRepository.FirstOrDefaultAsync(x => x.Id.Equals(id)) ?? throw new KeyNotFoundException("Department is not found");
            await _departmentRepository.SoftDeleteAsync(target);
        }
    }
}
