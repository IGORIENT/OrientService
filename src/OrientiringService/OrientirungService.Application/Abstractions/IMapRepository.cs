using OrientiringService.Domain.Maps;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrientiringService.Application.Abstractions
{
    public interface IMapRepository
    {
        Task<SportMap?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<IReadOnlyCollection<SportMap>> GetPublicMapsByUserIdAsync(Guid userId, CancellationToken cancellationToken);

        Task AddAsync(SportMap sportMap, CancellationToken cancellationToken);

        Task UpdateAsync(SportMap sportMap, CancellationToken cancellationToken);

        Task DeleteAsync(SportMap sportMap, CancellationToken cancellationToken);
    }
}
