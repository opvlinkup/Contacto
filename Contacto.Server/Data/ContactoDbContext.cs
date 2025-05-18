using Contacto.Server.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Contacto.Server.Data;

public class ContactoDbContext : DbContext
{
    public DbSet<Contact> Contacts { get; set; } = null!;

    public ContactoDbContext(DbContextOptions<ContactoDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureContact(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    private static void ConfigureContact(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasIndex(e => new { e.Name, e.MobilePhone })
                .HasDatabaseName("IX_Contacts_Name_MobilePhone").IsUnique();

            entity.Property(e => e.Name)
                .UseCollation("Latin1_General_CI_AS");
        });
    }
}