using Domain.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Services.Interfaces
{
    public interface IAuditLogService
    {
        Task<object> GetLog(int id, string entityName);
        Task<object> GetOriginalModel(int id, string entityName);
        Task TrackCreated(int id, string TableName, int userId);
        Task TrackUpdated<T>(T originalEntity, T updatedEntity, int userId, int id, string tableName);
    }
}
