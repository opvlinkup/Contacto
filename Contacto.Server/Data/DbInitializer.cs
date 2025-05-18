using Contacto.Server.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Contacto.Server.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ContactoDbContext>();

        await context.Database.MigrateAsync();

        if (context.Contacts.Any())
            return;

        var contacts = Enumerable.Range(1, 50).Select(i => new Contact
        {
            Id = Guid.NewGuid(),
            Name = $"User {i}",
            MobilePhone = $"37529165{i:00}",
            JobTitle = $"Manager {i:00}",
            BirthDate = DateTime.Today.AddYears(-25).AddDays(i * 30)
        });

        context.Contacts.AddRange(contacts);
        await context.SaveChangesAsync();
    }
}