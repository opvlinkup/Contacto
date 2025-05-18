using System.Reflection;
using Contacto.Server.Data.Repositories.Interfaces;
using Contacto.Server.Models.DTOs;
using Contacto.Server.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Contacto.Server.Data.Repositories.Implementation;

public class ContactRepository(ContactoDbContext context, ILogger<ContactRepository> logger) : IContactRepository
{
    private readonly ContactoDbContext _context = context;
    private readonly ILogger<ContactRepository> _logger = logger;

    public async Task AddAsync(Contact contact, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            var name = contact.Name.Trim().ToUpperInvariant();
            var phone = contact.MobilePhone.Trim();

            var existingContact = await _context.Contacts
                .FirstOrDefaultAsync(c =>
                        EF.Functions.Collate(c.Name, "Latin1_General_CI_AS") == name &&
                        c.MobilePhone == phone,
                    cancellationToken);

            if (existingContact != null)
                throw new InvalidOperationException(
                    $"Contact with name {contact.Name} and mobile phone {contact.MobilePhone} already exists");

            await _context.Contacts.AddAsync(contact, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("Contact with name {Name} and mobile phone {MobilePhone} successfully added",
                contact.Name, contact.MobilePhone);
        }
        catch (DbUpdateConcurrencyException e)
        {
            _logger.LogError(e, "Error adding contact: {Name} {MobilePhone}", contact.Name,
                contact.MobilePhone);
            throw;
        }
        catch (DbUpdateException e)
        {
            _logger.LogError(e, "Error adding contact: {Name} {MobilePhone}", contact.Name,
                contact.MobilePhone);
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error adding contact: {Name} {MobilePhone}", contact.Name,
                contact.MobilePhone);
            throw;
        }
    }


    public async Task<IEnumerable<Contact>> GetPaginatedAsync(
        int skip = 0,
        int take = 10,
        string? search = null,
        string? sortBy = null,
        bool ascending = true,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Contacts.AsNoTracking();

        try
        {
            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(c =>
                    EF.Functions.Like(c.Name, $"%{search}%"));


            query = sortBy?.ToLower() switch
            {
                "name" => ascending ? query.OrderBy(c => c.Name) : query.OrderByDescending(c => c.Name),
                "jobtitle" => ascending ? query.OrderBy(c => c.JobTitle) : query.OrderByDescending(c => c.JobTitle),
                "birthdate" => ascending ? query.OrderBy(c => c.BirthDate) : query.OrderByDescending(c => c.BirthDate),
                _ => query.OrderBy(c => c.Id)
            };


            query = query.Skip(skip).Take(take);

            return await query.ToListAsync(cancellationToken);
        }
        catch (OperationCanceledException e)
        {
            _logger.LogWarning(e, "Contact retrieval was canceled.");
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get paginated contacts.");
            throw;
        }
    }

    public async Task<Contact?> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Contacts
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id.ToString() == id, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to retrieve contact with ID: {Id}", id);
            throw new InvalidOperationException($"An error occurred while retrieving contact with ID {id}.", e);
        }
    }


    public async Task<IEnumerable<Contact>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Contacts
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get all contacts");
            throw;
        }
    }

    public async Task UpdateAsync(ContactUpdateDto contact, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            var existingContact = await _context.Contacts
                .FirstOrDefaultAsync(c => c.Id.ToString() == contact.Id, cancellationToken);

            if (existingContact is null)
            {
                _logger.LogWarning("Contact with id {Id} not found", contact.Id);
                throw new KeyNotFoundException($"Contact with id {contact.Id} not found");
            }

            ApplyUpdates(existingContact, contact, _logger);

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Contact with id {Id} successfully updated", contact.Id);
        }
        catch (DbUpdateException e)
        {
            _logger.LogError(e, "Error updating contact with id {Id}", contact.Id);
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error updating contact with id {Id}", contact.Id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            var contact = await _context.Contacts.FindAsync(new object[] { id }, cancellationToken);

            if (contact is null)
            {
                _logger.LogWarning("Contact with id {Id} not found", id);
                throw new KeyNotFoundException($"Contact with id {id} not found.");
            }

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Contact with id {Id} successfully deleted", id);
        }
        catch (DbUpdateConcurrencyException e)
        {
            _logger.LogError(e, "Concurrency error deleting contact with id {Id}", id);
            throw;
        }
        catch (DbUpdateException e)
        {
            _logger.LogError(e, "Database error deleting contact with id {Id}", id);
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error deleting contact with id {Id}", id);
            throw;
        }
    }

    /////////////////////////////////////////
    /// Helpers
    ////////////////////////////////////////
    private static void ApplyUpdates(Contact existing, ContactUpdateDto update, ILogger logger)
    {
        var propsToUpdate = typeof(ContactUpdateDto).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var existingProps = typeof(Contact).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .ToDictionary(p => p.Name);

        foreach (var prop in propsToUpdate)
        {
            var value = prop.GetValue(update);
            if (value == null) continue;

            if (!existingProps.TryGetValue(prop.Name, out var existingProp)) continue;
            if (!existingProp.CanWrite) continue;

            var targetType = Nullable.GetUnderlyingType(existingProp.PropertyType) ?? existingProp.PropertyType;

            try
            {
                object? safeValue;

                if (value is string strVal)
                {
                    strVal = strVal.Trim();
                    if (string.IsNullOrWhiteSpace(strVal)) continue;
                    safeValue = strVal;
                }
                else
                {
                    safeValue = Convert.ChangeType(value, targetType);
                }

                existingProp.SetValue(existing, safeValue);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to set property {prop.Name}: {ex.Message}");
                logger.LogError(ex, "Failed to set property {PropertyName} on contact", prop.Name);
            }
        }
    }
}