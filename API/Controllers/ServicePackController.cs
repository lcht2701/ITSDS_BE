﻿using API.DTOs.Requests.ServicePacks;
using API.DTOs.Requests.Services;
using AutoMapper;
using Domain.Constants;
using Domain.Exceptions;
using Domain.Models.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Repositories.Interfaces;

namespace API.Controllers
{
    [Route("/v1/itsds/servicepack")]
    public class ServicePackController : BaseController
    {
        private readonly IRepositoryBase<ServicePack> _servicePackRepository;

        public ServicePackController(IRepositoryBase<ServicePack> servicePackRepository)
        {
            _servicePackRepository = servicePackRepository;
        }

        [Authorize(Roles = Roles.MANAGER)]
        [HttpGet]
        public async Task<IActionResult> GetServicePack()
        {
            var result = await _servicePackRepository.ToListAsync();
            return Ok(result);
        }

        [Authorize(Roles = Roles.MANAGER)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetServicePackById(int id)
        {
            var result = await _servicePackRepository.FoundOrThrow(u => u.Id.Equals(id), new NotFoundException("Service Pack is not found"));
            return Ok(result);
        }

        [Authorize(Roles = Roles.MANAGER)]
        [HttpPost]
        public async Task<IActionResult> CreateServicePack([FromBody] CreateServicePackRequest model)
        {
            var entity = Mapper.Map(model, new ServicePack());
            await _servicePackRepository.CreateAsync(entity);
            return Ok(entity);
        }

        [Authorize(Roles = Roles.MANAGER)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServicePack(int id, [FromBody] UpdateServicePackRequest req)
        {
            var target = await _servicePackRepository.FoundOrThrow(c => c.Id.Equals(id), new NotFoundException("Service Pack not found"));
            ServicePack entity = Mapper.Map(req, target);
            await _servicePackRepository.UpdateAsync(entity);
            return Accepted("Updated Successfully");
        }

        [Authorize(Roles = Roles.MANAGER)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServicePack(int id)
        {
            var target = await _servicePackRepository.FoundOrThrow(c => c.Id.Equals(id), new NotFoundException("Service Pack not found"));
            //Soft Delete
            await _servicePackRepository.DeleteAsync(target);
            return Ok("Deleted Successfully");
        }
    }
}