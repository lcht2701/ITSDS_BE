

using API.DTOs.Responses.Dashboards.Managers.Tickets;

namespace API.Services.Interfaces
{
    public interface IExportService
    {
        Task<byte[]> ExportManagerAsync();

    }
}
