﻿using API.DTOs.Requests.Tickets;
using API.DTOs.Responses.Assignments;
using API.DTOs.Responses.Tickets;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Constants;
using Domain.Constants.Enums;
using Domain.Entities.Mails;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Contracts;
using Domain.Models.Tickets;
using Hangfire;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Persistence.Helpers;
using Persistence.Repositories.Interfaces;
using DateTime = System.DateTime;

namespace API.Services.Implements;

public class TicketService : ITicketService
{
    private readonly IRepositoryBase<Ticket> _ticketRepository;
    private readonly IRepositoryBase<Assignment> _assignmentRepository;
    private readonly IRepositoryBase<TicketTask> _taskRepository;
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IRepositoryBase<Service> _serviceRepository;
    private readonly IAuditLogService _auditLogService;
    private readonly IMessagingService _messagingService;
    private readonly IRepositoryBase<TeamMember> _teamMemberRepository;
    private readonly IRepositoryBase<CompanyMember> _companyMemberRepository;
    private readonly IRepositoryBase<Team> _teamRepository;
    private readonly IRepositoryBase<CompanyAddress> _CompanyAddressRepository;
    private readonly IAttachmentService _attachmentService;
    private readonly IMapper _mapper;
    private readonly MailSettings _mailSettings;

    public TicketService(IRepositoryBase<Ticket> ticketRepository, IRepositoryBase<Assignment> assignmentRepository,
        IRepositoryBase<TicketTask> taskRepository, IRepositoryBase<User> userRepository,
        IRepositoryBase<Service> serviceRepository, IAuditLogService auditLogService,
        IMessagingService messagingService, IRepositoryBase<TeamMember> teamMemberRepository, IMapper mapper,
        IOptions<MailSettings> mailSettings, IRepositoryBase<Team> teamRepository, IAttachmentService attachmentService,
        IRepositoryBase<CompanyMember> companyMemberRepository, IRepositoryBase<CompanyAddress> CompanyAddressRepository)
    {
        _ticketRepository = ticketRepository;
        _assignmentRepository = assignmentRepository;
        _taskRepository = taskRepository;
        _userRepository = userRepository;
        _serviceRepository = serviceRepository;
        _auditLogService = auditLogService;
        _messagingService = messagingService;
        _teamMemberRepository = teamMemberRepository;
        _teamRepository = teamRepository;
        _mapper = mapper;
        _mailSettings = mailSettings.Value;
        _attachmentService = attachmentService;
        _companyMemberRepository = companyMemberRepository;
        _CompanyAddressRepository = CompanyAddressRepository;
    }

    public async Task<List<GetTicketResponse>> Get()
    {
        var result = await _ticketRepository.GetAsync(navigationProperties: new string[]
            { "Requester", "Service", "Category", "Mode", "CreatedBy" });
        List<GetTicketResponse> response = await ModifyTicketListResponse(result);

        return response;
    }

    public async Task<GetTicketResponse> GetById(int id)
    {
        var result =
            await _ticketRepository.FirstOrDefaultAsync(x => x.Id.Equals(id),
                new string[] { "Requester", "Service", "Category", "Mode", "CreatedBy" }) ??
            throw new KeyNotFoundException("Ticket is not exist");
        var entity = _mapper.Map<Ticket, GetTicketResponse>(result);
        DataResponse.CleanNullableDateTime(entity);
        var ass = await _assignmentRepository.FirstOrDefaultAsync(x => x.TicketId.Equals(entity.Id),
            new string[] { "Team", "Technician" });
        if (ass != null)
        {
            var assMapping = _mapper.Map<GetAssignmentResponse>(ass);
            entity.Assignment = assMapping;
        }
        entity.AttachmentUrls = (await _attachmentService.Get(Tables.TICKET, entity.Id)).Select(x => x.Url).ToList();
        return entity;
    }

    public async Task<List<GetTicketResponse>> GetByUser(int userId)
    {
        List<Ticket> result = new();
        var companyMember = await _companyMemberRepository.FirstOrDefaultAsync(x => x.MemberId.Equals(userId), new string[] { "CompanyAddress" });
        if (companyMember == null)
        {
            result = (await _ticketRepository
                .WhereAsync(x => x.RequesterId.Equals(userId)))
                .ToList();
        }
        else
        {
            IEnumerable<int> memberIds;
            if (companyMember.IsCompanyAdmin)
            {
                memberIds = (await _companyMemberRepository
                    .WhereAsync(x =>
                        x.CompanyId.Equals(companyMember.CompanyId)))
                    .Select(x => x.MemberId);
            }
            else
            {
                memberIds = (await _companyMemberRepository
                    .WhereAsync(x =>
                        x.CompanyId.Equals(companyMember.CompanyId) &&
                        x.CompanyAddressId.Equals(companyMember.CompanyAddressId)))
                    .Select(x => x.MemberId);
            }

            result = (List<Ticket>)await _ticketRepository.WhereAsync(x => memberIds.Contains((int)x.RequesterId!),
                    new string[] { "Requester", "Service", "Category", "Mode", "CreatedBy" });
        }

        List<GetTicketResponse> response = await ModifyTicketListResponse(result);
        return response;
    }

    public async Task<List<GetTicketResponse>> GetTicketsOfTechnician(int userId)
    {
        var assignments = await _assignmentRepository.WhereAsync(x => x.TechnicianId == userId);
        var ticketIds = assignments.Select(assignment => assignment.TicketId).ToList();
        var result = await _ticketRepository.WhereAsync(ticket => ticketIds.Contains(ticket.Id),
            new string[] { "Requester", "Service", "Category", "Mode", "CreatedBy" });

        // Map the tickets to response entities
        List<GetTicketResponse> response = await ModifyTicketListResponse(result);

        return response;
    }

    public async Task<object> GetTicketLog(int id)
    {
        var result = await _auditLogService.GetLog(id, Tables.TICKET);
        return result;
    }

    public async Task<bool> IsTicketAssigned(int ticketId)
    {
        return await _assignmentRepository.FirstOrDefaultAsync(o => o.TicketId == ticketId) != null;
    }

    public async Task<Ticket> CreateByCustomer(int createdById, CreateTicketCustomerRequest model)
    {
        Ticket entity = _mapper.Map(model, new Ticket());
        entity.RequesterId = createdById;
        entity.CreatedById = createdById;
        entity.TicketStatus = TicketStatus.Open;
        entity.Address = GetCompanyAddressAddress((int)entity.RequesterId).Result;
        var categoryId = (await _serviceRepository.FirstOrDefaultAsync(x => x.Id.Equals(model.ServiceId))).CategoryId;
        if (categoryId != null) entity.CategoryId = (int)categoryId;
        var result = await _ticketRepository.CreateAsync(entity);
        if (model.AttachmentUrls != null)
        {
            await _attachmentService.Add(Tables.TICKET, result.Id, model.AttachmentUrls);
        }
        await AssignSupportJob(result);
        await CreateFirstTask(result);
        await SendNotificationAfterAssignment(result);
        return result;
    }

    public async Task<Ticket> CreateByManager(int createdById, CreateTicketManagerRequest model)
    {
        Ticket entity = _mapper.Map(model, new Ticket());
        entity.CreatedById = createdById;
        entity.TicketStatus = TicketStatus.Open;
        entity.Address = GetCompanyAddressAddress((int)entity.RequesterId!).Result;
        var result = await _ticketRepository.CreateAsync(entity);
        if (model.AttachmentUrls != null)
        {
            await _attachmentService.Add(Tables.TICKET, result.Id, model.AttachmentUrls);
        }

        if (model.TeamId != null && model.TechnicianId == null)
        {
            throw new BadRequestException("Technician cannot be empty when a team is selected.");
        }

        if (model.TechnicianId != null)
        {
            if (await IsTechnicianMemberOfTeamAsync(model.TechnicianId, model.TeamId) == null)
            {
                throw new BadRequestException("This technician is not a member of the specified team.");
            }

            var assignment = new Assignment()
            {
                TicketId = result.Id,
                TechnicianId = model.TechnicianId,
                TeamId = model.TeamId
            };

            await _assignmentRepository.CreateAsync(assignment);
            if (result.TicketStatus == TicketStatus.Open)
                await UpdateTicketStatus(result.Id, TicketStatus.Assigned);
        }
        else
        {
            await AssignSupportJob(result);
        }
        await CreateFirstTask(result);
        await SendNotificationAfterAssignment(result);
        return result;
    }

    public async Task<Ticket> UpdateByCustomer(int id, UpdateTicketCustomerRequest model)
    {
        var target =
            await _ticketRepository.FirstOrDefaultAsync(x => x.Id.Equals(id)) ?? throw new KeyNotFoundException();
        if (target.TicketStatus != TicketStatus.Open && target.TicketStatus != TicketStatus.Assigned)
        {
            throw new BadRequestException("Ticket cannot be edited");
        }
        var entity = _mapper.Map(model, target);
        var categoryId = (await _serviceRepository.FirstOrDefaultAsync(x => x.Id.Equals(model.ServiceId))).CategoryId;
        if (categoryId != null) target.CategoryId = (int)categoryId;
        var result = await _ticketRepository.UpdateAsync(entity);
        if (model.AttachmentUrls != null)
        {
            await _attachmentService.Update(Tables.TICKET, result.Id, model.AttachmentUrls);
        }
        return result;
    }

    public async Task<Ticket> UpdateByManager(int id, UpdateTicketManagerRequest model)
    {
        var target =
            await _ticketRepository.FirstOrDefaultAsync(x => x.Id.Equals(id)) ??
            throw new KeyNotFoundException("Ticket is not exist");
        var entity = _mapper.Map(model, target);
        var result = await _ticketRepository.UpdateAsync(entity);
        if (model.AttachmentUrls != null)
        {
            await _attachmentService.Update(Tables.TICKET, result.Id, model.AttachmentUrls);
        }
        return result;
    }

    public async Task<Ticket> UpdateByTechnician(int id, TechnicianAddDetailRequest model)
    {
        var target =
            await _ticketRepository.FirstOrDefaultAsync(x => x.Id.Equals(id)) ??
            throw new KeyNotFoundException("Ticket is not exist");
        var entity = _mapper.Map(model, target);
        var result = await _ticketRepository.UpdateAsync(entity);
        return result;
    }

    public async Task Remove(int id)
    {
        var target = await _ticketRepository.FirstOrDefaultAsync(x => x.Id.Equals(id)) ??
                     throw new KeyNotFoundException("Ticket is not exist");
        await _attachmentService.Delete(Tables.TICKET, target.Id);
        await _ticketRepository.SoftDeleteAsync(target);
    }

    public bool IsTicketDone(int? ticketId)
    {
        var ticket = _ticketRepository.FirstOrDefaultAsync(x => x.Id == ticketId).Result;
        return ticket.TicketStatus is TicketStatus.Closed or TicketStatus.Cancelled;
    }

    public async Task<Ticket> UpdateTicketStatus(int ticketId, TicketStatus newStatus)
    {
        var ticket = await _ticketRepository.FirstOrDefaultAsync(c => c.Id.Equals(ticketId)) ??
                     throw new KeyNotFoundException();
        ticket.TicketStatus = newStatus;
        if (newStatus == TicketStatus.Cancelled || newStatus == TicketStatus.Closed)
        {
            ticket.CompletedTime = DateTime.Now;
        }

        await _ticketRepository.UpdateAsync(ticket);
        return ticket;
    }

    public async Task<Ticket> ModifyTicketStatus(int ticketId, TicketStatus newStatus)
    {
        var ticket = await _ticketRepository.FirstOrDefaultAsync(c => c.Id.Equals(ticketId)) ??
                     throw new KeyNotFoundException("Ticket is not exist");
        var tasks = await _taskRepository.WhereAsync(x => x.TicketId.Equals(ticket.Id));
        var taskIncompletedCount = 0;
        taskIncompletedCount = tasks.Count(x =>
            x.TaskStatus != TicketTaskStatus.Completed &&
            x.TaskStatus != TicketTaskStatus.Cancelled);

        if (ticket.TicketStatus == TicketStatus.Open)
        {
            throw new BadRequestException("Ticket must be assigned before proceeding with further actions");
        }

        if (ticket.TicketStatus == TicketStatus.Closed || ticket.TicketStatus == TicketStatus.Cancelled)
        {
            throw new BadRequestException("Cannot update ticket status for a Closed or Cancelled ticket.");
        }

        if (newStatus == TicketStatus.Open || newStatus == TicketStatus.Closed || newStatus == TicketStatus.Cancelled)
        {
            throw new BadRequestException("Cannot set ticket status to Open, Closed, or Cancelled.");
        }

        switch (ticket.TicketStatus)
        {
            case TicketStatus.Assigned:
                if (newStatus == TicketStatus.InProgress)
                {
                    ticket.TicketStatus = newStatus;
                    await _ticketRepository.UpdateAsync(ticket);
                }
                else if (newStatus == TicketStatus.Resolved && taskIncompletedCount == 0)
                {
                    ticket.TicketStatus = newStatus;
                    await _ticketRepository.UpdateAsync(ticket);
                    AutoCloseBackgroundService(ticket);
                }
                else
                {
                    throw new BadRequestException("Cannot resovle ticket if all the tasks are not completed");
                }

                break;
            case TicketStatus.InProgress:
                if (newStatus == TicketStatus.Resolved && taskIncompletedCount == 0)
                {
                    ticket.TicketStatus = newStatus;
                    await _ticketRepository.UpdateAsync(ticket);
                    AutoCloseBackgroundService(ticket);
                }
                else
                {
                    throw new BadRequestException("Cannot resovle ticket if all the tasks are not completed");
                }
                break;
            case TicketStatus.Resolved:

                if (newStatus == TicketStatus.InProgress)
                {
                    ticket.TicketStatus = newStatus;
                    await _ticketRepository.UpdateAsync(ticket);
                    AutoCloseBackgroundService(ticket);
                }

                break;
            default:
                throw new BadRequestException();
        }

        return ticket;
    }

    public async Task<Ticket> CancelTicket(int ticketId, int userId)
    {
        var ticket = await _ticketRepository.FirstOrDefaultAsync(c => c.Id.Equals(ticketId)) ??
                     throw new KeyNotFoundException("Ticket is not exist");
        if (ticket.RequesterId == userId &&
            (ticket.TicketStatus == TicketStatus.Open || ticket.TicketStatus == TicketStatus.Assigned))
        {
            ticket.TicketStatus = TicketStatus.Cancelled;
            ticket.CompletedTime = DateTime.Now;
            await _ticketRepository.UpdateAsync(ticket);
            BackgroundJob.Enqueue(() => SendTicketMailNotification(ticket));
        }
        else
        {
            throw new BadRequestException(
                "Cancellation of the ticket is not allowed once it has entered the processing state");
        }

        return ticket;
    }

    public async Task<Ticket> CloseTicket(int ticketId, int userId)
    {
        var ticket = await _ticketRepository.FirstOrDefaultAsync(c => c.Id.Equals(ticketId)) ??
                     throw new KeyNotFoundException("Ticket is not exist");
        if (ticket.RequesterId == userId &&
            ticket.TicketStatus == TicketStatus.Resolved)
        {
            ticket.TicketStatus = TicketStatus.Closed;
            ticket.CompletedTime = DateTime.Now;
            await _ticketRepository.UpdateAsync(ticket);
            BackgroundJob.Enqueue(() => SendTicketMailNotification(ticket));
        }
        else
        {
            throw new BadRequestException(
                "If the ticket is not resolved, it cannot be closed");
        }

        return ticket;
    }

    public async Task SendNotificationAfterCloseTicket(Ticket ticket)
    {
        #region Notification

        var techinicianId = (await _assignmentRepository.FirstOrDefaultAsync(x => x.TicketId == ticket.Id))
            .TechnicianId;
        if (techinicianId != null)
        {
            await _messagingService.SendNotification("ITSDS", $"Ticket [{ticket.Title}] has been closed",
                (int)techinicianId);
        }

        if (ticket.RequesterId != null)
        {
            await _messagingService.SendNotification("ITSDS", $"Ticket [{ticket.Title}] has been closed",
                (int)ticket.RequesterId);
        }

        foreach (var managerId in await GetManagerIdsList())
        {
            await _messagingService.SendNotification("ITSDS", $"Ticket [{ticket.Title}] has been closed",
                managerId);
        }

        #endregion
        #region Mail Notification

        BackgroundJob.Enqueue(() => SendTicketMailNotification(ticket));

        #endregion
    }

    public async Task SendNotificationAfterAssignment(Ticket ticket)
    {
        var techinicianId = (await _assignmentRepository.FirstOrDefaultAsync(x => x.TicketId == ticket.Id))
            .TechnicianId;
        if (techinicianId != null)
        {
            await _messagingService.SendNotification("ITSDS", $"Ticket [{ticket.Title}] has been assigned to you",
                (int)techinicianId);
        }

        if (ticket.RequesterId != null)
        {
            await _messagingService.SendNotification("ITSDS", $"Ticket [{ticket.Title}] has been assigned",
                (int)ticket.RequesterId);
        }

        foreach (var managerId in await GetManagerIdsList())
        {
            await _messagingService.SendNotification("ITSDS", $"Ticket [{ticket.Title}] has been assigned",
                managerId);
        }
    }

    #region Get TicketStatus's List
    public Task<List<GetTicketStatusesResponse>> GetTicketStatuses()
    {
        var enumValues = Enum.GetValues(typeof(TicketStatus))
            .Cast<TicketStatus>()
            .Where(status => status != TicketStatus.Cancelled && status != TicketStatus.Closed)
            .Select(status => new GetTicketStatusesResponse
            {
                Id = (int)status,
                StatusName = DataResponse.GetEnumDescription((Enum?)status)
            })
            .ToList();

        return Task.FromResult(enumValues);
    }
    #endregion

    #region Background Services

    public async Task AssignSupportJob(Ticket ticket)
    {
        var teamIds = (await _teamRepository
            .WhereAsync(team => team.CategoryId == ticket.CategoryId))
            .Select(team => team.Id);

        if (!teamIds.Any())
            teamIds = (await _teamRepository
                .ToListAsync())
                .Select(team => team.Id);

        var memberIds = (await _teamMemberRepository
            .WhereAsync(teamMember => teamIds.Contains(teamMember.TeamId!)))
            .Select(teamMember => teamMember.MemberId);

        if (!memberIds.Any())
            memberIds = (await _teamMemberRepository.ToListAsync())
                .Select(teamMember => teamMember.MemberId);

        var availableTechnicians = await _userRepository
            .WhereAsync(user =>
                memberIds.Contains(user.Id) &&
                user.Role == Role.Technician &&
                user.IsActive == true);

        if (!availableTechnicians.Any())
        {
            // Handle the case where no technicians are available
            return;
        }

        int selectedTechnician = -1;
        int minValue = int.MaxValue;

        foreach (var technician in availableTechnicians)
        {
            int count = await GetNumberOfAssignmentsForTechnician(technician.Id);
            if (count < minValue)
            {
                minValue = count;
                selectedTechnician = technician.Id;
            }
        }

        if (selectedTechnician != -1)
        {
            var teamId = teamIds.First();
            var assignment = new Assignment()
            {
                TicketId = ticket.Id,
                TechnicianId = selectedTechnician,
                TeamId = teamId,
            };

            await _assignmentRepository.CreateAsync(assignment);
            await UpdateTicketStatus(ticket.Id, TicketStatus.Assigned);
        }
        else
        {
            // Handle the case where no technician is available
            // You might want to log this or take other actions
        }
    }

    public async Task CloseTicketJob(int ticketId)
    {
        await UpdateTicketStatus(ticketId, TicketStatus.Closed);
        var ticket = await _ticketRepository.FirstOrDefaultAsync(x => x.Id == ticketId);
        await SendNotificationAfterCloseTicket(ticket);
    }

    public async Task CancelCloseTicketJob(string jobId, int ticketId)
    {
        var ticket = await _ticketRepository.FirstOrDefaultAsync(x => x.Id.Equals(ticketId));
        if (ticket.TicketStatus != TicketStatus.Resolved)
        {
            BackgroundJob.Delete(jobId);
        }
    }

    #endregion

    #region Modify Ticket List Response
    private async Task<List<GetTicketResponse>> ModifyTicketListResponse(IEnumerable<Ticket> result)
    {
        var response = _mapper.Map<List<GetTicketResponse>>(result);
        foreach (var entity in response)
        {
            DataResponse.CleanNullableDateTime(entity);
            var ass = await _assignmentRepository.FirstOrDefaultAsync(x => x.TicketId.Equals(entity.Id),
                new string[] { "Team", "Technician" });
            if (ass != null)
            {
                var assMapping = _mapper.Map<GetAssignmentResponse>(ass);
                entity.Assignment = assMapping;
            }
            entity.AttachmentUrls = (await _attachmentService.Get(Tables.TICKET, entity.Id)).Select(x => x.Url).ToList();
        }

        return response;
    }
    #endregion

    #region Assignment Support

    public async Task<object> IsTechnicianMemberOfTeamAsync(int? technicianId, int? teamId)
    {
        var check = await _teamMemberRepository.FirstOrDefaultAsync(x =>
            x.MemberId.Equals(technicianId) && x.TeamId.Equals(teamId));

        return check;
    }

    #endregion

    public async Task CreateFirstTask(Ticket ticket)
    {
        var assignment = await _assignmentRepository
            .FirstOrDefaultAsync(x => x.TicketId.Equals(ticket.Id));
        var firstTask = new TicketTask()
        {
            TicketId = ticket.Id,
            CreateById = (int)assignment.TechnicianId!,
            Title = "Kiểm tra sơ bộ vấn đề",
            Description = "Tiến hành kiểm tra sơ bộ để đánh giá & đưa ra quy trình hỗ trợ",
            Note = "Nhiệm vụ mặc định",
            Priority = Domain.Constants.Enums.Priority.High,
            ScheduledStartTime = ticket.CreatedAt.Value,
            ScheduledEndTime = ticket.CreatedAt.Value.AddHours(1),
            Progress = 0,
            TaskStatus = TicketTaskStatus.New
        };
        await _taskRepository.CreateAsync(firstTask);
    }
    public async Task SendTicketMailNotification(Ticket ticket)
    {
        #region GetDetail

        var requester = await _userRepository.FirstOrDefaultAsync(x => x.Id.Equals(ticket.RequesterId))
                        ?? throw new KeyNotFoundException($"Requester with ID {ticket.RequesterId} is not exist");
        string requesterName = $"{requester.FirstName} {requester.LastName}";

        #endregion

        using (MimeMessage emailMessage = new MimeMessage())
        {
            MailboxAddress emailFrom = new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail);
            emailMessage.From.Add(emailFrom);
            MailboxAddress emailTo = new MailboxAddress(requesterName,
                requester.Email);
            emailMessage.To.Add(emailTo);

            emailMessage.Subject = "Ticket Notification";
            string emailTemplateText;
            switch (ticket.TicketStatus)
            {
                case TicketStatus.Closed:
                    emailTemplateText = @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Ticket Closure Notification</title>
</head>
<body style=""font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0;"">

    <div style=""max-width: 600px; margin: 20px auto; background-color: #fff; padding: 20px; border-radius: 5px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);"">

        <h2 style=""color: #333;"">Ticket Closure Notification</h2>

        <p>Dear {0},</p>
        <p>Your ticket has been successfully closed with the following details:</p>

        <ul style=""list-style-type: none; padding: 0;"">
            <li><strong>Title:</strong> {1}</li>
            <li><strong>Description:</strong> {2}</li>
            <li><strong>Status:</strong> Closed</li>
        </ul>

        <p>We appreciate your cooperation. If you have any further questions or concerns, please feel free to contact our support team.</p>

        <div style=""margin-top: 20px; text-align: center; color: #888;"">
            <p>This is an automated message. Please do not reply to this email.</p>
        </div>

    </div>

</body>
</html>
";
                    break;
                case TicketStatus.Cancelled:
                    emailTemplateText = @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Ticket Cancellation Notification</title>
</head>
<body style=""font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0;"">

    <div style=""max-width: 600px; margin: 20px auto; background-color: #fff; padding: 20px; border-radius: 5px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);"">

        <h2 style=""color: #333;"">Ticket Cancellation Notification</h2>

        <p>Dear {0},</p>
        <p>Your ticket has been successfully cancelled with the following details:</p>

        <ul style=""list-style-type: none; padding: 0;"">
            <li><strong>Title:</strong> {1}</li>
            <li><strong>Description:</strong> {2}</li>
            <li><strong>Status:</strong> Cancelled</li>
        </ul>

        <p>If you have any questions or concerns, please feel free to contact our support team.</p>

        <div style=""margin-top: 20px; text-align: center; color: #888;"">
            <p>This is an automated message. Please do not reply to this email.</p>
        </div>

    </div>

</body>
</html>
";
                    break;
                default:
                    return;
            }

            emailTemplateText = string.Format(emailTemplateText, requesterName, ticket.Title, ticket.Description);

            BodyBuilder emailBodyBuilder = new BodyBuilder();
            emailBodyBuilder.HtmlBody = emailTemplateText;
            emailBodyBuilder.TextBody = "Plain Text goes here to avoid marked as spam for some email servers.";

            emailMessage.Body = emailBodyBuilder.ToMessageBody();

            using (SmtpClient mailClient = new SmtpClient())
            {
                await mailClient.ConnectAsync(_mailSettings.Server, _mailSettings.Port,
                    SecureSocketOptions.StartTls);
                await mailClient.AuthenticateAsync(_mailSettings.SenderEmail, _mailSettings.Password);
                await mailClient.SendAsync(emailMessage);
                await mailClient.DisconnectAsync(true);
            }
        }
    }
    private string AutoCloseBackgroundService(Ticket ticket)
    {
        //Auto Schedule Job to Close Ticket
        string jobId = BackgroundJob.Schedule(
            () => CloseTicketJob(ticket.Id),
            TimeSpan.FromDays(2));
        BackgroundJob.ContinueJobWith(
            jobId, () => SendNotificationAfterCloseTicket(ticket));
        RecurringJob.AddOrUpdate(
            jobId + "_Cancellation",
            () => CancelCloseTicketJob(jobId, ticket.Id),
            "*/5 * * * * *"); //Every 5
        return jobId;
    }
    private async Task<int> GetNumberOfAssignmentsForTechnician(int technicianId)
    {
        var result = await _assignmentRepository.WhereAsync(x => x.TechnicianId == technicianId);
        return result.Count;
    }
    private async Task<List<int>> GetManagerIdsList()
    {
        var managerIds = (await _userRepository.WhereAsync(x => x.Role == Role.Manager)).Select(x => x.Id).ToList();
        return managerIds;
    }
    private async Task<string> GetCompanyAddressAddress(int memberId)
    {
        var member = await _companyMemberRepository.FirstOrDefaultAsync(x => x.MemberId == memberId);

        if (member == null)
        {
            return "";
        }

        var companyAddress = await _CompanyAddressRepository.FirstOrDefaultAsync(x => x.CompanyId == member.CompanyId);
        return companyAddress?.Address ?? "";
    }

}