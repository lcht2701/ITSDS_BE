using API.DTOs.Requests.Modes;
using API.Services.Interfaces;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Helpers;

namespace API.Controllers
{
    [Route("/v1/itsds/mode")]
    public class ModeController : BaseController
    {
        private readonly IModeService _modeService;

        public ModeController(IModeService modeService)
        {
            _modeService = modeService;
        }

        [Authorize]
        [HttpGet("all")]

        public async Task<IActionResult> GetAllMode()
        {
            var result = await _modeService.Get();
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
            var result = await _modeService.Get();
            var pagedResponse = result.AsQueryable().GetPagedData(page, pageSize, filter, sort);
            return Ok(pagedResponse);
        }

        [Authorize(Roles = Roles.ADMIN)]
        [HttpGet("{modeId}")]
        public async Task<IActionResult> GetModeById(int modeId)
        {
            try
            {
                var result = await _modeService.GetById(modeId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = Roles.ADMIN)]
        [HttpPost]
        public async Task<IActionResult> CreateMode([FromBody] CreateModeRequest model)
        {
            try
            {
                var entity = await _modeService.Create(model);
                return Ok(new
                {
                    Message = "Mode Created Successfully",
                    Data = entity
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = Roles.ADMIN)]
        [HttpPut("{modeId}")]
        public async Task<IActionResult> UpdateMode(int modeId, [FromBody] UpdateModeRequest model)
        {
            try
            {
                var entity = await _modeService.Update(modeId, model);
                return Ok(new
                {
                    Message = "Mode Updated Successfully",
                    Data = entity
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = Roles.ADMIN)]
        [HttpDelete("{modeId}")]
        public async Task<IActionResult> DeleteMode(int modeId)
        {
            try
            {
                await _modeService.Remove(modeId);
                return Ok("Mode Deleted Successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
