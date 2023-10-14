
using API.DTOs.Requests.Contracts;
using Domain.Constants;
using Domain.Exceptions;
using Domain.Models.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Repositories.Interfaces;

namespace API.Controllers
{
    [Route("/v1/itsds/contract")]
    public class ContractController : BaseController
    {
        private readonly IRepositoryBase<Contract> _contractRepository;

        public ContractController(IRepositoryBase<Contract> contractRepository)
        {
            _contractRepository = contractRepository;
        }

        [Authorize(Roles = Roles.ACCOUNTANT)]
        [HttpGet]
        public async Task<IActionResult> GetContracts()
        {
            var result = await _contractRepository.ToListAsync();
            return Ok(result);
        }

        [Authorize(Roles = Roles.ACCOUNTANT)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContractById(int id)
        {
            var result = await _contractRepository.FoundOrThrow(u => u.Id.Equals(id), new NotFoundException("Contract is not found"));
            return Ok(result);
        }

        [Authorize(Roles = Roles.ACCOUNTANT)]
        [HttpPost]
        public async Task<IActionResult> CreateContract([FromBody] CreateContractRequest model)
        {
            var entity = Mapper.Map(model, new Contract());
            await _contractRepository.CreateAsync(entity);
            return Ok(entity);
        }

        [Authorize(Roles = Roles.ACCOUNTANT)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContract(int id, [FromBody] UpdateContractRequest req)
        {
            var target = await _contractRepository.FoundOrThrow(c => c.Id.Equals(id), new NotFoundException("Contract not found"));
            Contract entity = Mapper.Map(req, target);
            await _contractRepository.UpdateAsync(entity);
            return Accepted("Update Successfully");
        }

        [Authorize(Roles = Roles.ACCOUNTANT)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContract(int id)
        {
            var target = await _contractRepository.FoundOrThrow(c => c.Id.Equals(id), new NotFoundException("Contract not found"));
            //Soft Delete
            await _contractRepository.DeleteAsync(target);
            return Ok("Delete Successfully");
        }

    }
}
