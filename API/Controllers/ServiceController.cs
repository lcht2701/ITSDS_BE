﻿
using API.DTOs.Requests.Services;
using Domain.Constants;
using Domain.Exceptions;
using Domain.Models.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Helpers;
using Persistence.Repositories.Interfaces;
using System.Linq;

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

        [Authorize]
        [HttpGet("/all")]

        public async Task<IActionResult> GetAllService()
        {
            var result = await _serviceRepository.ToListAsync();
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetServices(
        [FromQuery] string? filter,
        [FromQuery] string? sort,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5)
        {
            var result = await _serviceRepository.ToListAsync();
            var pagedResponse = result.AsQueryable().GetPagedData(page, pageSize, filter, sort);
            return Ok(pagedResponse);
        }

        [Authorize(Roles = Roles.MANAGER)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceById(int id)
        {
            var result = await _serviceRepository.FirstOrDefaultAsync(u => u.Id.Equals(id), new string[] { "ServicePack" });
            return result != null ? Ok(result) : throw new BadRequestException("Service not found");
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
            var target = await _serviceRepository.FoundOrThrow(c => c.Id.Equals(id), new BadRequestException("Service not found"));
            Service entity = Mapper.Map(req, target);
            await _serviceRepository.UpdateAsync(entity);
            return Accepted("Update Successfully");
        }

        [Authorize(Roles = Roles.MANAGER)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var target = await _serviceRepository.FoundOrThrow(c => c.Id.Equals(id), new BadRequestException("Service not found"));
            await _serviceRepository.SoftDeleteAsync(target);
            return Ok("Delete Successfully");
        }
    }
}
