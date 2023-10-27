using API.DTOs.Requests.Feedbacks;
using API.DTOs.Responses.Feedbacks;
using API.DTOs.Responses.TicketSolutions;
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
            .WhereAsync(x => x.SolutionId.Equals(solutionId), new string[] { "TicketSolution", "User" });

        if (user.Role == Role.Customer)
        {
            feedbackQuery = (IList<Feedback>)feedbackQuery.Where(x => x.IsPublic == true);
        }

        var result = feedbackQuery.ToList() ?? throw new KeyNotFoundException();
        var response = result.Select(feedback =>
        {
            var entity = _mapper.Map<GetFeedbackResponse>(feedback);
            DataResponse.CleanNullableDateTime(entity);
            return entity;
        }).ToList();

        return response;
    }


    public async Task Create(CreateFeedbackRequest model, int userId)
    {
        var user = await _userRepository.FirstOrDefaultAsync(x => x.Id.Equals(userId));
        var solution = await _solutionRepository.FirstOrDefaultAsync(x => x.Id.Equals(model.SolutionId)) ?? throw new KeyNotFoundException("solution is not exist");
        var entity = _mapper.Map(model, new Feedback());
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
