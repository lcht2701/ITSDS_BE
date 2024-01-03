using API.DTOs.Requests.TicketSolutions;
using API.DTOs.Responses.TicketSolutions;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Constants;
using Domain.Constants.Enums;
using Domain.Models;
using Domain.Models.Tickets;
using Org.BouncyCastle.Tsp;
using Persistence.Helpers;
using Persistence.Repositories.Interfaces;
using static Grpc.Core.Metadata;

namespace API.Services.Implements;

public class TicketSolutionService : ITicketSolutionService
{
    private readonly IRepositoryBase<TicketSolution> _solutionRepository;
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IRepositoryBase<Reaction> _reactRepository;
    private readonly IAttachmentService _attachmentService;
    private readonly IMapper _mapper;

    public TicketSolutionService(IRepositoryBase<TicketSolution> solutionRepository, IRepositoryBase<User> userRepository, IRepositoryBase<Reaction> reactRepository, IAttachmentService attachmentService, IMapper mapper)
    {
        _solutionRepository = solutionRepository;
        _userRepository = userRepository;
        _reactRepository = reactRepository;
        _attachmentService = attachmentService;
        _mapper = mapper;
    }

    public async Task<List<GetTicketSolutionResponse>> Get(int userId)
    {
        var user = await _userRepository.FirstOrDefaultAsync(x => x.Id.Equals(userId));

        var result = await _solutionRepository
            .GetAsync(navigationProperties: new string[] { "Category", "Owner", "CreatedBy" });

        if (user.Role == Role.Customer)
        {
            result = result.Where(x => x.IsPublic == true && x.IsApproved == true);
        }

        List<GetTicketSolutionResponse> response = new();
        foreach (var item in result)
        {
            var entity = _mapper.Map<GetTicketSolutionResponse>(item);
            DataResponse.CleanNullableDateTime(entity);
            //logic reaction
            entity.CountLike = (await _reactRepository.WhereAsync(x => x.SolutionId == entity.Id && x.ReactionType == 0 )).Count;
            entity.CountDislike = (await _reactRepository.WhereAsync(x => x.SolutionId == entity.Id && x.ReactionType == 1)).Count;
            var currentReactionUser = await _reactRepository.FirstOrDefaultAsync(x => x.SolutionId == entity.Id && x.UserId == userId);
            if(currentReactionUser == null)
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

    public async Task<object> GetById(int id, int userId)   
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
        entity.CreatedById = createdById;
        entity.IsApproved = false;
        var result = await _solutionRepository.CreateAsync(entity);
        if (model.AttachmentUrls != null)
        {
            await _attachmentService.Add(Tables.TICKETSOLUTION, result.Id, model.AttachmentUrls);
        }
    }

    public async Task Update(int solutionId, UpdateTicketSolutionRequest model)
    {
        var target = await _solutionRepository.FirstOrDefaultAsync(c => c.Id.Equals(solutionId)) ??
                     throw new KeyNotFoundException();
        var entity = _mapper.Map(model, target);
        var result = await _solutionRepository.UpdateAsync(entity);
        if (model.AttachmentUrls != null)
        {
            await _attachmentService.Add(Tables.TICKETSOLUTION, result.Id, model.AttachmentUrls);
        }
    }

    public async Task Remove(int solutionId)
    {
        var target = await _solutionRepository.FirstOrDefaultAsync(c => c.Id.Equals(solutionId)) ??
                     throw new KeyNotFoundException();
        await _attachmentService.Delete(Tables.TICKETSOLUTION, target.Id);
        await _solutionRepository.SoftDeleteAsync(target);
    }

    public async Task Approve(int solutionId)
    {
        var target = await _solutionRepository.FirstOrDefaultAsync(c => c.Id.Equals(solutionId)) ??
                     throw new KeyNotFoundException();
        target.IsApproved = true;
        await _solutionRepository.UpdateAsync(target);
    }

    public async Task Reject(int solutionId)
    {
        var target = await _solutionRepository.FirstOrDefaultAsync(c => c.Id.Equals(solutionId)) ??
                     throw new KeyNotFoundException();
        target.IsApproved = false;
        await _solutionRepository.UpdateAsync(target);
    }

    public async Task SubmitForApproval(int solutionId)
    {
        var target = await _solutionRepository.FirstOrDefaultAsync(c => c.Id.Equals(solutionId)) ??
                     throw new KeyNotFoundException();
        target.IsApproved = !target.IsApproved;
        await _solutionRepository.UpdateAsync(target);
    }

    public async Task ChangePublic(int solutionId)
    {
        var target = await _solutionRepository.FirstOrDefaultAsync(c => c.Id.Equals(solutionId)) ??
                     throw new KeyNotFoundException();
        target.IsPublic = !target.IsPublic;
        await _solutionRepository.UpdateAsync(target);
    }
}