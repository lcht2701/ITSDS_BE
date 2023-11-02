using API.Services.Interfaces;
using Domain.Constants;
using Domain.Exceptions;
using Domain.Models;
using Domain.Models.Tickets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.X509;
using Persistence.Context;
using Persistence.Helpers;
using Persistence.Repositories.Interfaces;
using System.Reflection;
using static Domain.Customs.CustomAttributes;

namespace API.Services.Implements;

public class AuditLogService : IAuditLogService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IRepositoryBase<Ticket> _ticketRepository;

    public AuditLogService(ApplicationDbContext dbContext, IRepositoryBase<Ticket> ticketRepository)
    {
        _dbContext = dbContext;
        _ticketRepository = ticketRepository;
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

        if (originalEntity.GetType() != updatedEntity.GetType())
        {
            throw new InvalidOperationException("originalEntity and updatedEntity must be of the same type.");
        }

        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var currentTime = DateTime.UtcNow;
        List<AuditLog> historyEntries = new List<AuditLog>();

        foreach (var property in properties)
        {
            var originalValue = property.GetValue(originalEntity);
            var updatedValue = property.GetValue(updatedEntity);

            // Check if the property has the ExcludeFromAuditLog attribute
            if (Attribute.IsDefined(property, typeof(ExcludeFromAuditLogAttribute)))
            {
                continue; // Skip this property
            }

            if (!Equals(originalValue, updatedValue))
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

                historyEntries.Add(historyEntry);
            }
        }

        await _dbContext.AuditLogs.AddRangeAsync(historyEntries);
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

    public async Task<object> GetLog(int id, string entityName)
    {
        var logs = await _dbContext.AuditLogs
            .Where(log => log.EntityName.Equals(entityName) && log.EntityRowId == id)
            .Include(log => log.User)
            .ToListAsync();

        var groupedHistories = logs
            .GroupBy(log => new { log.Timestamp, log.User?.Username, log.Action })
            .OrderByDescending(group => group.Key.Timestamp)
            .Select(group => new
            {
                group.Key.Timestamp,
                group.Key.Username,
                group.Key.Action,
                Entries = group.Select(entry => new
                {
                    entry.Id,
                    entry.Message
                }).ToList()
            }).ToList();

        return groupedHistories;
    }

    public async Task<object> GetOriginalModel(int id, string entityName)
    {
        switch (entityName)
        {
            case "Ticket":
                var entity = await _ticketRepository.FirstOrDefaultAsync(x => x.Id.Equals(id));
                var original = JsonConvert.DeserializeObject<Ticket>(JsonConvert.SerializeObject(entity));
                return original;

            default:
                return null;
        }
    }


}

