using API.Services.Interfaces;
using Domain.Models;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements;

public class AttachmentService : IAttachmentService
{
    private readonly IRepositoryBase<Attachment> _repo;

    public AttachmentService(IRepositoryBase<Attachment> repo)
    {
        _repo = repo;
    }

    public async Task<List<Attachment>> Get(string table, int rowId)
    {
        var result = await _repo.WhereAsync(x => x.Table.Equals(table) && x.RowId == rowId);
        return result.ToList();
    }

    public async Task<List<Attachment>> Add(string table, int rowId, List<string> attachments)
    {
        List<Attachment> newList = new();
        foreach (var attachment in attachments)
        {
            Attachment newAttachment = new Attachment()
            {
                Table = table,
                RowId = rowId,
                Url = attachment
            };
            newList.Add(newAttachment);
        }
        await _repo.CreateAsync(newList);
        return newList;
    }

    public async Task<List<Attachment>> Update(string table, int rowId, List<string> attachments)
    {
        var target = await _repo.WhereAsync(x => x.Table.Equals(table) && x.RowId == rowId);
        foreach(var entity in target)
        {
            await _repo.DeleteAsync(entity);
        }

        List<Attachment> newList = new();
        foreach (var attachment in attachments)
        {
            Attachment newAttachment = new Attachment()
            {
                Table = table,
                RowId = rowId,
                Url = attachment
            };
            newList.Add(newAttachment);
        }
        await _repo.CreateAsync(newList);
        return newList;
    }

    public async Task Delete(string table, int rowId)
    {
        var target = await _repo.WhereAsync(x => x.Table.Equals(table) && x.RowId == rowId);
        foreach (var entity in target)
        {
            await _repo.DeleteAsync(entity);
        }
    }
}
