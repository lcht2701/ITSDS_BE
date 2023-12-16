using API.DTOs.Requests.Feedbacks;
using API.DTOs.Responses.Feedbacks;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Constants.Enums;
using Domain.Models;
using Domain.Models.Tickets;
using Persistence.Helpers;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class FeedbackService : IFeedbackService
{
    private readonly IRepositoryBase<Feedback> _feedbackRepository;
    private readonly IRepositoryBase<TicketSolution> _solutionRepository;
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IMapper _mapper;

    public FeedbackService(IRepositoryBase<Feedback> feedbackRepository, IRepositoryBase<TicketSolution> solutionRepository, IRepositoryBase<User> userRepository, IMapper mapper)
    {
        _feedbackRepository = feedbackRepository;
        _solutionRepository = solutionRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<List<GetFeedbackResponse>> Get(int solutionId, int userId)
    {
        var user = await _userRepository.FirstOrDefaultAsync(x => x.Id.Equals(userId));

        var feedbackQuery = await _feedbackRepository
            .WhereAsync(x => x.SolutionId.Equals(solutionId) && x.ParentFeedbackId == null, new string[] { "User" });

        if (user.Role == Role.Customer)
        {
            feedbackQuery = feedbackQuery.Where(x => x.IsPublic == true).ToList();
        }

        var result = feedbackQuery.ToList() ?? throw new KeyNotFoundException();

        var response = _mapper.Map<List<GetFeedbackResponse>>(result);
        foreach (var feedback in response)
        {
            feedback.FeedbackReplies = await GetRepliesRecursive(feedback.Id);
        }
        return response;
    }

    private async Task<List<GetFeedbackResponse>> GetRepliesRecursive(int parentFeedbackId)
    {
        var replies = await _feedbackRepository.WhereAsync(x => x.ParentFeedbackId == parentFeedbackId, new string[] { "User" });
        if (replies.Count == 0)
        {
            return new List<GetFeedbackResponse>();
        }

        var response = _mapper.Map<List<GetFeedbackResponse>>(replies);
        foreach (var reply in response)
        {
            reply.FeedbackReplies = await GetRepliesRecursive(reply.Id);
        }
        return response;
    }

    public async Task<object> GetById(int id)
    {
        var feedback = await _feedbackRepository
            .FirstOrDefaultAsync(x => x.Id.Equals(id), new string[] { "TicketSolution", "User" }) ?? throw new KeyNotFoundException("Feedback is not exist");
        if (feedback.ParentFeedbackId == null)
        {
            var response = _mapper.Map(feedback, new GetFeedbackResponse());
            var replies = await _feedbackRepository.WhereAsync(x => x.ParentFeedbackId == feedback.Id);
            response.FeedbackReplies = replies.Select(r => _mapper.Map<GetFeedbackResponse>(r)).ToList();
            return response;
        }
        else
        {
            var reply = _mapper.Map(feedback, new GetFeedbackResponse());
            return reply;
        }
    }

    public async Task<object> Create(CreateFeedbackRequest model, int userId)
    {
        var user = await _userRepository.FirstOrDefaultAsync(x => x.Id.Equals(userId));
        var solution = await _solutionRepository.FirstOrDefaultAsync(x => x.Id.Equals(model.SolutionId)) ?? throw new KeyNotFoundException("Solution does not exist");
        var entity = _mapper.Map(model, new Feedback());
        entity.UserId = userId;
        if (user.Role == Role.Customer)
        {
            entity.IsPublic = true;
        }
        await _feedbackRepository.CreateAsync(entity);
        return entity;
    }

    public async Task<object> CreateReply(CreateReplyRequest model, int userId)
    {
        var user = await _userRepository.FirstOrDefaultAsync(x => x.Id.Equals(userId));
        var parentFeedback = await _feedbackRepository.FirstOrDefaultAsync(x => x.Id.Equals(model.ParentFeedbackId)) ?? throw new KeyNotFoundException("Parent feedback is not found");

        var entity = _mapper.Map(model, new Feedback());
        entity.UserId = userId;
        entity.SolutionId = parentFeedback.SolutionId;
        entity.ParentFeedbackId = model.ParentFeedbackId;

        if (user.Role == Role.Customer)
        {
            entity.IsPublic = true;
        }
        await _feedbackRepository.CreateAsync(entity);
        return entity;
    }

    public async Task<object> Update(int id, UpdateFeedbackRequest model, int userId)
    {
        var user = await _userRepository.FirstOrDefaultAsync(x => x.Id.Equals(userId));
        var target = await _feedbackRepository.FirstOrDefaultAsync(c => c.Id.Equals(id)) ?? throw new KeyNotFoundException("Feedback is not found");
        Feedback entity = _mapper.Map(model, target);
        if (user.Role == Role.Customer)
        {
            entity.IsPublic = true;
        }
        await _feedbackRepository.UpdateAsync(entity);
        return entity;
    }

    public async Task Delete(int id)
    {
        var target = await _feedbackRepository.FirstOrDefaultAsync(c => c.Id.Equals(id)) ?? throw new KeyNotFoundException("Feedback is not found");
        if (target.ParentFeedbackId == null)
        {
            var replies = await _feedbackRepository.WhereAsync(x => x.Id.Equals(target.ParentFeedbackId));
            foreach (var reply in replies)
            {
                await _feedbackRepository.SoftDeleteAsync(reply);
            }
        }
        await _feedbackRepository.SoftDeleteAsync(target);
    }

    public async Task<List<GetFeedbackResponse>> GetReplies(int feedbackId)
    {
        var replies = await _feedbackRepository.WhereAsync(x => x.ParentFeedbackId == feedbackId);

        var response = replies.Select(reply =>
        {
            var entity = _mapper.Map<GetFeedbackResponse>(reply);
            DataResponse.CleanNullableDateTime(entity);
            return entity;
        }).ToList();

        return response;
    }

}
