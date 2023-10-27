using API.DTOs.Requests.TicketSolutions;
using API.DTOs.Responses.TicketSolutions;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Constants.Enums;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Tickets;
using Org.BouncyCastle.Ocsp;
using Persistence.Helpers;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class TicketSolutionService : ITicketSolutionService
{
    private readonly IRepositoryBase<TicketSolution> _solutionRepository;
    private readonly IRepositoryBase<User> _userRepository;
    private readonly IMapper _mapper;

    public TicketSolutionService(IRepositoryBase<TicketSolution> solutionRepository,
        IRepositoryBase<User> userRepository, IMapper mapper)
    {
        _solutionRepository = solutionRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<List<GetTicketSolutionResponse>> Get(int userId)
    {
        var user = await _userRepository.FirstOrDefaultAsync(x => x.Id.Equals(userId));

        var result = await _solutionRepository
            .GetAsync(navigationProperties: new string[] { "Category", "Owner" });

        if (user.Role == Role.Customer)
        {
            result = result.Where(x => x.IsPublic == true && x.IsApproved == true);
        }

        var response = result.Select(solution =>
        {
            var entity = _mapper.Map<GetTicketSolutionResponse>(solution);
            DataResponse.CleanNullableDateTime(entity);
            return entity;
        }).ToList();

        return response;
    }


    public async Task<object> GetById(int id)
    {
        var result = await _solutionRepository.FirstOrDefaultAsync(x => x.Id.Equals(id),
            new string[] { "Category", "Owner" }) ?? throw new KeyNotFoundException();
        var response = _mapper.Map(result, new GetTicketSolutionResponse());
        return response;
    }

    public async Task Create(CreateTicketSolutionRequest model)
    {
        var entity = _mapper.Map(model, new TicketSolution());
        await _solutionRepository.CreateAsync(entity);
    }

    public async Task Update(int solutionId, UpdateTicketSolutionRequest model)
    {
        var target = await _solutionRepository.FirstOrDefaultAsync(c => c.Id.Equals(solutionId)) ??
                     throw new KeyNotFoundException();
        var entity = _mapper.Map(model, target);
        await _solutionRepository.UpdateAsync(entity);
    }

    public async Task Remove(int solutionId)
    {
        var target = await _solutionRepository.FirstOrDefaultAsync(c => c.Id.Equals(solutionId)) ??
                     throw new KeyNotFoundException();
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
        //handle later
    }

    public async Task ChangePublic(int solutionId)
    {
        var target = await _solutionRepository.FirstOrDefaultAsync(c => c.Id.Equals(solutionId)) ??
                     throw new KeyNotFoundException();
        target.IsPublic = !target.IsPublic;
        await _solutionRepository.UpdateAsync(target);
    }
}