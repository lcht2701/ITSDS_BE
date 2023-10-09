using API.DTOs.Requests.Categories;
using API.DTOs.Requests.TicketAnalysts;
using Domain.Constants;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Tickets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistence.Repositories.Interfaces;

namespace API.Controllers;

[Route("/v1/itsds/ticket/analyst")]
public class TicketAnalystController : BaseController
{
    private readonly IRepositoryBase<TicketAnalyst> _analystRepository;

    public TicketAnalystController(IRepositoryBase<TicketAnalyst> analystRepository)
    {
        _analystRepository = analystRepository;
    }

    [Authorize(Roles = $"{Roles.CUSTOMER},{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpGet]
    public async Task<IActionResult> GetAnalystsOfTicket(int ticketId)
    {
        var result = await _analystRepository.WhereAsync(x => x.TicketId.Equals(ticketId));
        if (result.Count == 0)
        {
            return Ok("No Analysts");
        }
        return Ok(result);
    }

    [Authorize(Roles = $"{Roles.CUSTOMER},{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpGet("{analystId}")]
    public async Task<IActionResult> GetAnalystById(int analystId)
    {
        var result = await _analystRepository.FoundOrThrow(x => x.Id.Equals(analystId), new NotFoundException("Analyst not found"));
        return Ok(result);
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpPost("new-analyst")]
    public async Task<IActionResult> CreateAnalyst(int ticketId, [FromBody] CreateTicketAnalystRequest model)
    {
        var entity = Mapper.Map(model, new TicketAnalyst());
        entity.TicketId = ticketId;
        await _analystRepository.CreateAsync(entity);
        return Ok("Created Successfully");
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateAnalyst(int ticketId, int analystId, [FromBody] UpdateTicketAnalystRequest req)
    {
        var target = await _analystRepository.FoundOrThrow(c => c.Id.Equals(analystId) && c.TicketId.Equals(ticketId), new NotFoundException("Analyst not found"));
        TicketAnalyst entity = Mapper.Map(req, target);
        await _analystRepository.UpdateAsync(entity);
        return Ok("Updated Successfully");
    }

    [Authorize(Roles = $"{Roles.MANAGER},{Roles.TECHNICIAN}")]
    [HttpDelete("{categoryId}")]
    public async Task<IActionResult> DeleteAnalyst(int ticketId, int analystId)
    {
        var target = await _analystRepository.FoundOrThrow(c => c.Id.Equals(analystId) && c.TicketId.Equals(ticketId), new NotFoundException("Analyst not found"));
        //Soft Delete
        await _analystRepository.DeleteAsync(target);
        return Ok("Deleted Successfully");
    }
}
