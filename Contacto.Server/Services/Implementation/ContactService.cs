using Contacto.Server.Data.Repositories.Interfaces;
using Contacto.Server.Models.DTOs;
using Contacto.Server.Models.Entities;
using Contacto.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Contacto.Server.Services.Implementation;

public class ContactService(IContactRepository contactRepository, ILogger<ContactService> logger)
    : IContactService
{
    public async Task<IEnumerable<Contact>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await contactRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<Contact> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            logger.LogWarning("Provided contact id is null or whitespace.");
            throw new ArgumentException("Id cannot be null or whitespace.", nameof(id));
        }

        if (!Guid.TryParse(id, out _))
        {
            logger.LogWarning("Provided contact id is not a valid GUID: {Id}", id);
            throw new ArgumentException("Invalid GUID format.", nameof(id));
        }

        try
        {
            var contact = await contactRepository.GetAsync(id, cancellationToken);

            if (contact == null) throw new KeyNotFoundException($"Contact with id {id} not found.");

            return contact;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<IEnumerable<Contact>> GetPaginatedAsync(int skip, int take, string? search, string? sortBy,
        bool ascending, CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Attempting to retrieve paginated contacts. Skip: {Skip}, Take: {Take}, Search: '{Search}', SortBy: '{SortBy}', Ascending: {Ascending}",
            skip, take, search ?? "N/A", sortBy ?? "N/A", ascending);


        try
        {
            var contacts =
                await contactRepository.GetPaginatedAsync(skip, take, search, sortBy, ascending, cancellationToken);
            var contactList = contacts?.ToList() ?? new List<Contact>();

            logger.LogInformation("Successfully retrieved {Count} contacts.", contactList.Count);
            return contactList;
        }
        catch (OperationCanceledException e)
        {
            logger.LogWarning(e, "Contact retrieval was canceled. Parameters - Skip: {Skip}, Take: {Take}", skip,
                take);
            throw;
        }
        catch (Exception e)
        {
            logger.LogError(e,
                "An error occurred while retrieving paginated contacts." +
                " Parameters - Skip: {Skip}, Take: {Take}, Search: '{Search}', SortBy: '{SortBy}', Ascending: {Ascending}",
                skip, take, search, sortBy, ascending);
            throw;
        }
    }

    public async Task AddAsync(Contact contact, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(contact, nameof(contact));
        ArgumentException.ThrowIfNullOrEmpty(contact.Name, nameof(contact.Name));
        ArgumentException.ThrowIfNullOrEmpty(contact.MobilePhone, nameof(contact.MobilePhone));

        try
        {
            await contactRepository.AddAsync(contact, cancellationToken);
            logger.LogInformation("Contact {ContactId} added successfully", contact.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to add contact {ContactName}", contact.Name);
            throw;
        }
    }

    public async Task<Contact> UpdateAsync(ContactUpdateDto updateDto, CancellationToken cancellationToken = default)
    {
        if (updateDto == null) throw new ArgumentNullException(nameof(updateDto));
        
        if (string.IsNullOrWhiteSpace(updateDto.Id))
        {
            logger.LogWarning("Provided contact update id is null or whitespace.");
            throw new ArgumentException("Id cannot be null or whitespace.", nameof(updateDto.Id));
        }

        if (!Guid.TryParse(updateDto.Id, out _))
        {
            logger.LogWarning("Provided contact update id is not a valid GUID: {Id}", updateDto.Id);
            throw new ArgumentException("Invalid GUID format.", nameof(updateDto.Id));
        }

        try
        {
            await contactRepository.UpdateAsync(updateDto, cancellationToken);
            var updatedContact = await contactRepository.GetAsync(updateDto.Id, cancellationToken) ?? throw new KeyNotFoundException($"Contact with id {updateDto.Id} not found after update.");
            return updatedContact;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task DeleteAsync(Guid guid, CancellationToken cancellationToken = default)
    {
        try
        {
            await contactRepository.DeleteAsync(guid, cancellationToken);
        }

        catch (DbUpdateConcurrencyException e)
        {
            logger.LogError(e, "Concurrency error deleting contact with guid {Id}", guid);
            throw;
        }
        catch (DbUpdateException e)
        {
            logger.LogError(e, "Database error deleting contact with guid {Id}", guid);
            throw;
        }

        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}