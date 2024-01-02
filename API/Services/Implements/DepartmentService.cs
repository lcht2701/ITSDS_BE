using API.DTOs.Requests.Departments;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Models.Contracts;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IRepositoryBase<Department> _departmentRepository;
        private readonly IMapper _mapper;

        public DepartmentService(IRepositoryBase<Department> departmentRepository, IMapper mapper)
        {
            _departmentRepository = departmentRepository;
            _mapper = mapper;
        }

        public async Task<List<Department>> GetByCompany(int companyId)
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
