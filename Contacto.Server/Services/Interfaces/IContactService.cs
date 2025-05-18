using Contacto.Server.Models.DTOs;
using Contacto.Server.Models.Entities;

namespace Contacto.Server.Services.Interfaces;

public interface IContactService
{
    Task<IEnumerable<Contact>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Contact> GetAsync(string id, CancellationToken cancellationToken = default);

    Task<IEnumerable<Contact>> GetPaginatedAsync(int skip, int take, string? search, string? sortBy, bool ascending,
        CancellationToken cancellationToken = default);

    Task AddAsync(Contact contact, CancellationToken cancellationToken = default);
    Task<Contact> UpdateAsync(ContactUpdateDto updateDto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid guid, CancellationToken cancellationToken = default);
}