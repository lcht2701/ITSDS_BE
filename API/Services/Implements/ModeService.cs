using API.DTOs.Requests.Modes;
using API.Services.Interfaces;
using AutoMapper;
using Domain.Models.Tickets;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class ModeService : IModeService
{
    private readonly IRepositoryBase<Mode> _moderepository;
    private readonly IMapper _mapper;

    public ModeService(IRepositoryBase<Mode> moderepository, IMapper mapper)
    {
        _moderepository = moderepository;
        _mapper = mapper;
    }

    public async Task<List<Mode>> Get()
    {
        return await _moderepository.ToListAsync();
    }

    public async Task<Mode> GetById(int id)
    {
        var result = await _moderepository.FoundOrThrow(x => x.Id.Equals(id), new KeyNotFoundException("Mode is not exist"));
        return result;
    }

    public async Task<Mode> Create(CreateModeRequest model)
    {
        var entity = _mapper.Map(model, new Mode());
        await _moderepository.CreateAsync(entity);
        return entity;
    }

    public async Task<Mode> Update(int id, UpdateModeRequest model)
    {
        var target = await _moderepository.FoundOrThrow(c => c.Id.Equals(id), new KeyNotFoundException("Mode is not exist"));
        Mode entity = _mapper.Map(model, target);
        await _moderepository.UpdateAsync(entity);
        return entity;
    }

    public async Task Remove(int id)
    {
        var target = await _moderepository.FoundOrThrow(c => c.Id.Equals(id), new KeyNotFoundException("Mode is not exist"));
        await _moderepository.SoftDeleteAsync(target);
    }
}
