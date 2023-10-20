using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Services.Interfaces
{
    public interface IAuditLogService
    {
        Task TrackCreated(int id, string TableName, int userId);
        Task TrackUpdated<T>(T originalEntity, T updatedEntity, int userId, int id, string tableName);
    }
}
