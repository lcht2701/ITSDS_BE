using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Tickets;
using Persistence.Repositories.Interfaces;
using Persistence.Services.Interfaces;

namespace Persistence.Services.Implements;

public class AssignmentService : IAssignmentService
{
    private readonly IRepositoryBase<Ticket> _ticketRepository;
    private readonly IRepositoryBase<Assignment> _assignmentRepository;

    public AssignmentService(IRepositoryBase<Ticket> ticketRepository, IRepositoryBase<Assignment> assignmentRepository)
    {
        _ticketRepository = ticketRepository;
        _assignmentRepository = assignmentRepository;
    }

    public async Task<bool> AssignTechnicianToTicket(int ticketId, int newTechnicianId)
    {
        try
        {
            // Get the ticket
            Ticket ticket = await _ticketRepository.FoundOrThrow(
                o => o.Id.Equals(ticketId),
                new NotFoundException("Ticket not found")
            );

            // Check if the ticket is already assigned to a technician
            var existingAssignment = await _assignmentRepository.FirstOrDefaultAsync(
                x => x.TicketId.Equals(ticketId)
            );

            if (existingAssignment != null)
            {
                throw new BadRequestException("Ticket is already assigned to a technician.");
            }

            // Create a new assignment for the technician
            var newAssignment = new Assignment()
            {
                TicketId = ticketId,
                TechnicianId = newTechnicianId,
            };

            await _assignmentRepository.CreateAsync(newAssignment);

            return true;
        }
        catch (NotFoundException)
        {
            throw; // Re-throw the exception as-is
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while processing the request.", ex);
        }
    }


    public async Task<bool> UpdateTechnicianAssignment(int ticketId, int newTechnicianId)
    {
        try
        {
            // Check if the new technician is already assigned
            var existingAssignment = await _assignmentRepository.FirstOrDefaultAsync(
                x => x.TicketId.Equals(ticketId) && x.TechnicianId != newTechnicianId
            );

            if (existingAssignment != null)
            {
                // Delete the previous assignment if it exists
                await _assignmentRepository.DeleteAsync(existingAssignment);
            }

            // Create a new assignment for the technician
            var newAssignment = new Assignment()
            {
                TicketId = ticketId,
                TechnicianId = newTechnicianId,
            };

            await _assignmentRepository.CreateAsync(newAssignment);

            return true; // Return true to indicate success
        }
        catch (Exception ex)
        {
            // Log the exception for debugging purposes
            // You might also want to return a more informative error message
            throw new Exception("An error occurred while updating technician assignment.", ex);
        }
    }

}
