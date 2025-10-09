using Microsoft.EntityFrameworkCore;
using SubscriptionService.Models;

namespace SubscriptionService.Data;

public static class DbInitializer
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        // Create database if it doesn't exist
        await context.Database.EnsureCreatedAsync();
        
        // Seed test service if not exists
        if (!await context.Services.AnyAsync())
        {
            context.Services.Add(new Service 
            { 
                ServiceId = "test_service",
                Password = "test_password"
            });
            await context.SaveChangesAsync();
        }
    }
}
