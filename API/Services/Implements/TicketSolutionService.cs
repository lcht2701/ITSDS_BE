using API.DTOs.Requests.TicketSolutions;
using API.DTOs.Responses.TicketSolutions;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Application.AppConfig;
using Domain.Constants;
using Domain.Constants.Enums;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Tickets;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Persistence.Helpers;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class TicketSolutionService : ITicketSolutionService
{
    private readonly IRepositoryBase<TicketSolution> _solutionRepository;
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IRepositoryBase<Reaction> _reactRepository;
    private readonly IRepositoryBase<TeamMember> _teamMemberRepository;
    private readonly IAttachmentService _attachmentService;
    private readonly IMapper _mapper;
    private readonly MailSettings _mailSettings;

    public TicketSolutionService(IRepositoryBase<TicketSolution> solutionRepository, IRepositoryBase<User> userRepository, IRepositoryBase<Reaction> reactRepository, IAttachmentService attachmentService, IMapper mapper, IOptions<MailSettings> mailSettings, IRepositoryBase<TeamMember> teamMemberRepository)
    {
        _solutionRepository = solutionRepository;
        _userRepository = userRepository;
        _reactRepository = reactRepository;
        _attachmentService = attachmentService;
        _mapper = mapper;
        _mailSettings = mailSettings.Value;
        _teamMemberRepository = teamMemberRepository;
    }

    public async Task<List<GetTicketSolutionResponse>> Get(int userId)
    {
        var user = await _userRepository.FirstOrDefaultAsync(x => x.Id.Equals(userId));

        var result = await _solutionRepository
            .GetAsync(navigationProperties: new string[] { "Category", "Owner", "CreatedBy" });

        if (user.Role == Role.Customer)
        {
            result = result.Where(x => x.IsApproved == true);
        }

        List<GetTicketSolutionResponse> response = new();
        foreach (var item in result)
        {
            var entity = _mapper.Map<GetTicketSolutionResponse>(item);
            DataResponse.CleanNullableDateTime(entity);
            //logic reaction
            entity.CountLike = (await _reactRepository.WhereAsync(x => x.SolutionId == entity.Id && x.ReactionType == 0)).Count;
            entity.CountDislike = (await _reactRepository.WhereAsync(x => x.SolutionId == entity.Id && x.ReactionType == 1)).Count;
            var currentReactionUser = await _reactRepository.FirstOrDefaultAsync(x => x.SolutionId == entity.Id && x.UserId == userId);
            if (currentReactionUser == null)
            {
                entity.CurrentReactionUser = null;
            }
            else
            {
                entity.CurrentReactionUser = currentReactionUser.ReactionType;
            }
            //done logic
            entity.AttachmentUrls = (await _attachmentService.Get(Tables.TICKETSOLUTION, entity.Id)).Select(x => x.Url).ToList();
            response.Add(entity);
        }

        return response;
    }

    public async Task<List<GetTicketSolutionResponse>> GetUnapprovedSolutions(int userId)
    {
        var result = await _solutionRepository
            .WhereAsync(x => x.IsApproved == false,
                new string[] { "Category", "Owner", "CreatedBy" });

        List<GetTicketSolutionResponse> response = new();
        foreach (var item in result)
        {
            var entity = _mapper.Map<GetTicketSolutionResponse>(item);
            DataResponse.CleanNullableDateTime(entity);
            //logic reaction
            entity.CountLike = (await _reactRepository.WhereAsync(x => x.SolutionId == entity.Id && x.ReactionType == 0)).Count;
            entity.CountDislike = (await _reactRepository.WhereAsync(x => x.SolutionId == entity.Id && x.ReactionType == 1)).Count;
            var currentReactionUser = await _reactRepository.FirstOrDefaultAsync(x => x.SolutionId == entity.Id && x.UserId == userId);
            if (currentReactionUser == null)
            {
                entity.CurrentReactionUser = null;
            }
            else
            {
                entity.CurrentReactionUser = currentReactionUser.ReactionType;
            }
            //done logic
            entity.AttachmentUrls = (await _attachmentService.Get(Tables.TICKETSOLUTION, entity.Id)).Select(x => x.Url).ToList();
            response.Add(entity);
        }

        return response;
    }

    public async Task<GetTicketSolutionResponse> GetById(int id, int userId)
    {
        var result = await _solutionRepository.FirstOrDefaultAsync(x => x.Id.Equals(id),
            new string[] { "Category", "Owner", "CreatedBy" }) ?? throw new KeyNotFoundException();
        var response = _mapper.Map(result, new GetTicketSolutionResponse());
        response.CountLike = (await _reactRepository.WhereAsync(x => x.SolutionId == response.Id && x.ReactionType == 0)).Count;
        response.CountDislike = (await _reactRepository.WhereAsync(x => x.SolutionId == response.Id && x.ReactionType == 1)).Count;
        var currentReactionUser = await _reactRepository.FirstOrDefaultAsync(x => x.SolutionId == response.Id && x.UserId == userId);
        if (currentReactionUser == null)
        {
            response.CurrentReactionUser = null;
        }
        else
        {
            response.CurrentReactionUser = currentReactionUser.ReactionType;
        }
        response.AttachmentUrls = (await _attachmentService.Get(Tables.TICKETSOLUTION, response.Id)).Select(x => x.Url).ToList();
        return response;
    }

    public async Task Create(CreateTicketSolutionRequest model, int createdById)
    {
        var entity = _mapper.Map(model, new TicketSolution());
        var currentUser = await _userRepository.FirstOrDefaultAsync(x => x.Id == createdById);
        entity.CreatedById = createdById;
        entity.IsApproved = currentUser.Role is Role.Manager;
        entity.ReviewDate = entity.IsApproved == true ? DateTime.Now : null;
        var result = await _solutionRepository.CreateAsync(entity);
        if (model.AttachmentUrls != null)
        {
            await _attachmentService.Add(Tables.TICKETSOLUTION, result.Id, model.AttachmentUrls);
        }
    }

    public async Task Update(int solutionId, UpdateTicketSolutionRequest model, int userId)
    {
        var target = await _solutionRepository.FirstOrDefaultAsync(c => c.Id.Equals(solutionId)) ??
                     throw new KeyNotFoundException();
        await IsTechnicianTheCreator(target, userId);
        var entity = _mapper.Map(model, target);
        var result = await _solutionRepository.UpdateAsync(entity);
        if (model.AttachmentUrls != null)
        {
            await _attachmentService.Add(Tables.TICKETSOLUTION, result.Id, model.AttachmentUrls);
        }
    }

    public async Task Remove(int solutionId, int userId)
    {
        var target = await _solutionRepository.FirstOrDefaultAsync(c => c.Id.Equals(solutionId)) ??
                     throw new KeyNotFoundException();
        await IsTechnicianTheCreator(target, userId);
        await _attachmentService.Delete(Tables.TICKETSOLUTION, target.Id);
        await _solutionRepository.SoftDeleteAsync(target);
    }

    public async Task Approve(int solutionId)
    {
        var target = await _solutionRepository.FirstOrDefaultAsync(c => c.Id.Equals(solutionId)) ??
                     throw new KeyNotFoundException();
        target.IsApproved = true;
        target.ReviewDate = DateTime.Now;
        await _solutionRepository.UpdateAsync(target);
    }

    public async Task Reject(int solutionId)
    {
        var target = await _solutionRepository.FirstOrDefaultAsync(c => c.Id.Equals(solutionId)) ??
                     throw new KeyNotFoundException();
        target.IsApproved = false;
        target.ReviewDate = null;
        await _solutionRepository.UpdateAsync(target);
    }

    public async Task SubmitForApproval(int solutionId, int userId, int managerId)
    {
        var target = await _solutionRepository.FirstOrDefaultAsync(x => x.Id == solutionId, new string[] { "CreatedBy" }) ?? throw new KeyNotFoundException();
        var manager = await _userRepository.FirstOrDefaultAsync(x => x.Id == managerId);
        await IsTechnicianTheCreator(target, userId);
        using (MimeMessage emailMessage = new MimeMessage())
        {
            string managerName = $"{manager.FirstName} {manager.LastName}";
            string createrName = $"{target.CreatedBy!.FirstName} {target.CreatedBy!.LastName}";
            MailboxAddress emailFrom = new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail);
            emailMessage.From.Add(emailFrom);
            MailboxAddress emailTo = new MailboxAddress(managerName,
                manager.Email);
            emailMessage.To.Add(emailTo);

            emailMessage.Subject = "Approval Request For Ticket Solution";
            string emailTemplateText = @"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Approval Request: Ticket Solution</title>
</head>
<body style=""font-family: 'Arial', sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0;"">

    <div style=""max-width: 600px; margin: 20px auto; background-color: #ffffff; border-radius: 5px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);"">
        <div style=""background-color: #007bff; color: #fff; text-align: center; padding: 20px; border-top-left-radius: 5px; border-top-right-radius: 5px;"">
            <h2 style=""margin: 0;"">Approval Request: Ticket Solution</h2>
        </div>

        <div style=""padding: 20px;"">
            <p>Hello {0},</p>
            <p>The following ticket solution requires your approval:</p>

            <p><strong>Title:</strong> {1}</p>
            <p><strong>Created by:</strong> {2}</p>
            <p><strong>Content:</strong> {3}</p>

            <p style=""background-color: #ffeb3b; padding: 5px; font-weight: bold;"">Please review the solution for appearance on Ticket Solution List</p>


        </div>

        <div style=""background-color: #f8f9fa; text-align: center; padding: 10px; border-bottom-left-radius: 5px; border-bottom-right-radius: 5px;"">
            <p>Thank you,</p>
            <p>ITSDS</p>
        </div>
    </div>

</body>
</html>
";
            emailTemplateText = string.Format(emailTemplateText, managerName, target.Title, createrName, target.Content);

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

    private async Task IsTechnicianTheCreator(TicketSolution solution, int userId)
    {
        var currentUser = await _userRepository.FirstOrDefaultAsync(x => x.Id.Equals(userId));
        if (currentUser.Role == Role.Technician && !solution.CreatedById.Equals(currentUser.Id))
        {
            throw new BadRequestException("You do not have permission on solutions created by others.");
        }
    }
}