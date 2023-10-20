using Domain.Models;
using Domain.Models.Tickets;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Persistence.Context;
using Persistence.Helpers;
using Persistence.Repositories.Interfaces;
using Persistence.Services.Interfaces;
using System.Reflection;
using static Domain.Customs.CustomAttributes;

namespace Persistence.Services.Implements;

public class AuditLogService : IAuditLogService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IRepositoryBase<User> _userRepository;
    public AuditLogService(ApplicationDbContext dbContext, IRepositoryBase<User> userRepository)
    {
        _dbContext = dbContext;
        _userRepository = userRepository;
    }

    public async Task TrackCreated(int id, string TableName, int userId)
    {
        var historyEntry = new AuditLog
        {
            Action = "Created",
            UserId = userId,
            EntityName = TableName,
            EntityRowId = id,
            Timestamp = DateTime.UtcNow,
        };

        await _dbContext.AuditLogs.AddAsync(historyEntry);
        await _dbContext.SaveChangesAsync();
    }

    public async Task TrackUpdated<T>(T originalEntity, T updatedEntity, int userId, int id, string tableName)
    {
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var currentTime = DateTime.UtcNow;

        foreach (var property in properties)
        {
            var originalValue = property.GetValue(originalEntity);
            var updatedValue = property.GetValue(updatedEntity);

            // Check if the property has the ExcludeFromAuditLog attribute
            if (Attribute.IsDefined(property, typeof(ExcludeFromAuditLogAttribute)))
            {
                continue; // Skip this property
            }

            if (!object.Equals(originalValue, updatedValue))
            {
                string message = GetUpdateMessage(property.Name, updatedValue);

                var historyEntry = new AuditLog
                {
                    UserId = userId,
                    EntityName = tableName,
                    EntityRowId = id,
                    Action = "Updated",
                    Message = message,
                    Timestamp = currentTime,
                };

                await _dbContext.AuditLogs.AddAsync(historyEntry);
            }
        }

        await _dbContext.SaveChangesAsync();
    }

    private static string GetUpdateMessage(string propertyName, object updatedValue)
    {
        if (updatedValue is Enum enumValue)
        {
            return $"{propertyName} updated to {DataResponse.GetEnumDescription(enumValue)}";
        }
        else
        {
            return $"{propertyName} updated to \"{updatedValue}\"";
        }
    }

}

