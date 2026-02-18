using Microsoft.EntityFrameworkCore;
using SchoolSystem.Backend.Data;
using SchoolSystem.Domain.Entities;

namespace SchoolSystem.Backend.Services;

public class SystemSettingsService(SchoolDbContext context)
{
    public async Task<SystemSettings> GetAsync()
    {
        return await context.SystemSettings.FirstAsync();
    }

    public async Task<SystemSettings> UpdateAsync(SystemSettings settings)
    {
        context.SystemSettings.Update(settings);
        await context.SaveChangesAsync();
        return settings;
    }
}