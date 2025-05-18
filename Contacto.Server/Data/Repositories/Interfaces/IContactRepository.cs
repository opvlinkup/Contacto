using Contacto.Server.Models.DTOs;
using Contacto.Server.Models.Entities;

namespace Contacto.Server.Data.Repositories.Interfaces;

public interface IContactRepository
{
    Task<IEnumerable<Contact>> GetPaginatedAsync(
        int skip = 0,
        int take = 20,
        string? search = null,
        string? sortBy = null,
        bool ascending = true,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Contact>> GetAllAsync(CancellationToken cancellationToken);
    Task<Contact?> GetAsync(string id, CancellationToken cancellation);
    Task AddAsync(Contact contact, CancellationToken cancellationToken);
    Task UpdateAsync(ContactUpdateDto contact, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}