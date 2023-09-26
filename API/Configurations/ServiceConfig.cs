using AutoWrapper;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace API.Configurations;

public static class ServiceConfig
{
    public static async Task ApplyMigrations(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        await using ApplicationDbContext dbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}
