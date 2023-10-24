using API.DTOs.Requests.Categories;
using API.DTOs.Requests.Modes;
using AutoMapper;
using Domain.Constants;
using Domain.Exceptions;
using Domain.Models.Tickets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Utilities.IO;
using Persistence.Helpers;
using Persistence.Repositories.Interfaces;

namespace API.Controllers
{
    [Route("/v1/itsds/mode")]
    public class ModeController : BaseController
    {
        private readonly IRepositoryBase<Mode> _moderepository;

        public ModeController(IRepositoryBase<Mode> moderepository)
        {
            _moderepository = moderepository;
        }

        [Authorize]
        [HttpGet("all")]

        public async Task<IActionResult> GetAllMode()
        {
            var result = await _moderepository.ToListAsync();
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetModes(
        [FromQuery] string? filter,
        [FromQuery] string? sort,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5)
        {
            var result = await _moderepository.ToListAsync();
            var pagedResponse = result.AsQueryable().GetPagedData(page, pageSize, filter, sort);
            return Ok(pagedResponse);
        }

        [Authorize(Roles = Roles.ADMIN)]
        [HttpGet("{modeId}")]
        public async Task<IActionResult> GetModeById(int modeId)
        {
            var result = await _moderepository.FoundOrThrow(x => x.Id.Equals(modeId), new BadRequestException("Mode not found"));
            return Ok(result);
        }

        [Authorize(Roles = Roles.ADMIN)]
        [HttpPost]
        public async Task<IActionResult> CreateMode([FromBody] CreateModeRequest model)
        {
            var entity = Mapper.Map(model, new Mode());
            await _moderepository.CreateAsync(entity);
            return Ok();
        }

        [Authorize(Roles = Roles.ADMIN)]
        [HttpPut("{modeId}")]
        public async Task<IActionResult> UpdateMode(int modeId, [FromBody] UpdateModeRequest req)
        {
            var target = await _moderepository.FoundOrThrow(c => c.Id.Equals(modeId), new BadRequestException("Mode not found"));
            Mode entity = Mapper.Map(req, target);
            await _moderepository.UpdateAsync(entity);
            return Accepted("Update Successfully");
        }

        [Authorize(Roles = Roles.ADMIN)]
        [HttpDelete("{modeId}")]
        public async Task<IActionResult> DeleteMode(int modeId)
        {
            var target = await _moderepository.FoundOrThrow(c => c.Id.Equals(modeId), new BadRequestException("Mode not found"));
            await _moderepository.SoftDeleteAsync(target);
            return Ok("Delete Successfully");
        }
    }
}
