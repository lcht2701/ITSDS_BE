using Domain.Models;

namespace API.Services.Interfaces
{
    public interface IAttachmentService
    {
        Task<List<Attachment>> Get(string table, int rowId);
        Task<List<Attachment>> Add(string table, int rowId, List<string>? attachments);
        Task<List<Attachment>> Update(string table, int rowId, List<string>? attachments);
        Task Delete(string table, int rowId);
    }
}
