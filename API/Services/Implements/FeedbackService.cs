using API.DTOs.Requests.Feedbacks;
using API.DTOs.Responses.Feedbacks;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Constants.Enums;
using Domain.Models;
using Domain.Models.Tickets;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class FeedbackService : IFeedbackService
{
    private readonly IRepositoryBase<Feedback> _feedbackRepository;
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IMapper _mapper;

    public FeedbackService(IRepositoryBase<Feedback> feedbackRepository, IRepositoryBase<User> userRepository, IMapper mapper)
    {
        _feedbackRepository = feedbackRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<List<GetFeedbackResponse>> Get(int solutionId)
    {
        var result = await _feedbackRepository.WhereAsync(x => x.SolutionId.Equals(solutionId),
            new string[] { "TicketSolution", "User" }) ?? throw new KeyNotFoundException();
        var response = _mapper.Map<List<GetFeedbackResponse>>(result);
        return response;
    }

    public async Task<List<GetFeedbackCustomerResponse>> GetByCustomer(int solutionId)
    {
        var result = await _feedbackRepository.WhereAsync(x =>
            x.SolutionId.Equals(solutionId) &&
            x.IsPublic == true,
            new string[] { "TicketSolution", "User" }) ?? throw new KeyNotFoundException();
        var response = _mapper.Map<List<GetFeedbackCustomerResponse>>(result);
        return response;
    }

    public async Task Create(int solutionId, CreateFeedbackRequest model, int userId)
    {
        var user = await _userRepository.FirstOrDefaultAsync(x => x.Id.Equals(userId));
        var entity = _mapper.Map(model, new Feedback());
        entity.SolutionId = solutionId;
        entity.UserId = userId;
        if (user.Role == Role.Customer)
        {
            entity.IsPublic = true;
        }
        await _feedbackRepository.CreateAsync(entity);
    }

    public async Task Update(int id, UpdateFeedbackRequest model, int userId)
    {
        var user = await _userRepository.FirstOrDefaultAsync(x => x.Id.Equals(userId));
        var target = await _feedbackRepository.FirstOrDefaultAsync(c => c.Id.Equals(id)) ?? throw new KeyNotFoundException();
        Feedback entity = _mapper.Map(model, target);
        if (user.Role == Role.Customer)
        {
            entity.IsPublic = true;
        }
        await _feedbackRepository.UpdateAsync(entity);
    }

    public async Task Delete(int id)
    {
        var target = await _feedbackRepository.FirstOrDefaultAsync(c => c.Id.Equals(id)) ?? throw new KeyNotFoundException();
        await _feedbackRepository.SoftDeleteAsync(target);
    }
}
