
using API.DTOs.Requests.Services;
using Domain.Constants;
using Domain.Exceptions;
using Domain.Models.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Repositories.Interfaces;

namespace API.Controllers
{
    [Route("/v1/itsds/service")]
    public class ServiceController : BaseController
    {
        private readonly IRepositoryBase<Service> _serviceRepository;

        public ServiceController(IRepositoryBase<Service> serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        [Authorize(Roles = Roles.MANAGER)]
        [HttpGet]
        public async Task<IActionResult> GetServices()
        {
            var result = await _serviceRepository.ToListAsync();
            return Ok(result);
        }

        [Authorize(Roles = Roles.MANAGER)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceById(int id)
        {
            var result = await _serviceRepository.FoundOrThrow(u => u.Id.Equals(id), new NotFoundException("Service is not found"));
            return Ok(result);
        }

        [Authorize(Roles = Roles.MANAGER)]
        [HttpPost]
        public async Task<IActionResult> CreateService([FromBody] CreateServiceRequest model)
        {
            var entity = Mapper.Map(model, new Service());
            await _serviceRepository.CreateAsync(entity);
            return Ok(entity);
        }

        [Authorize(Roles = Roles.MANAGER)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(int id, [FromBody] UpdateServiceRequest req)
        {
            var target = await _serviceRepository.FoundOrThrow(c => c.Id.Equals(id), new NotFoundException("Service not found"));
            Service entity = Mapper.Map(req, target);
            await _serviceRepository.UpdateAsync(entity);
            return Accepted("Updated Successfully");
        }

        [Authorize(Roles = Roles.MANAGER)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var target = await _serviceRepository.FoundOrThrow(c => c.Id.Equals(id), new NotFoundException("Service not found"));
            //Soft Delete
            await _serviceRepository.DeleteAsync(target);
            return Ok("Deleted Successfully");
        }
    }
}
