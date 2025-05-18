using Contacto.Server.Data;
using Contacto.Server.Data.Repositories.Implementation;
using Contacto.Server.Data.Repositories.Interfaces;
using Contacto.Server.Extensions;
using Contacto.Server.Services.Implementation;
using Contacto.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Contacto.Server;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<ContactoDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowLocal", policy =>
                policy.WithOrigins("https://localhost:46532")
                    .WithMethods("GET", "POST", "PATCH", "DELETE")
                    .WithHeaders("Content-Type", "Accept")
                    .DisallowCredentials()
            );
        });

        builder.Services.AddScoped<IContactService, ContactService>();
        builder.Services.AddScoped<IContactRepository, ContactRepository>();

        builder.Services.AddControllers();

        var app = builder.Build();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        // Configure the HTTP request pipeline.

        app.UseHttpsRedirection();
        app.UseSecurityHeaders();

        app.UseCors("AllowLocal");

        app.UseAuthorization();


        app.MapControllers();

        app.MapFallbackToFile("/index.html");

        await DbInitializer.InitializeAsync(app.Services, app.Configuration);

        app.Run();
    }
}