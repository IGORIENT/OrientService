using OrienteeringService.Domain.Maps;

namespace OrienteeringService.Application.Abstractions
{
    public interface IMapRepository
    {
        Task<SportMap?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<SportMap?> GetByIdAndOwnerIdAsync(
            Guid id,
            Guid ownerId,
            CancellationToken cancellationToken);

        Task<IReadOnlyCollection<SportMap>> GetByOwnerIdAsync(
            Guid ownerId,
            CancellationToken cancellationToken);

        Task<IReadOnlyCollection<SportMap>> GetPublicMapsByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken);

        Task AddAsync(SportMap sportMap, CancellationToken cancellationToken);

        Task UpdateAsync(SportMap sportMap, CancellationToken cancellationToken);

        Task DeleteAsync(SportMap sportMap, CancellationToken cancellationToken);
    }
}
