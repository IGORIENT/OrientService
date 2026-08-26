using Microsoft.EntityFrameworkCore;
using OrienteeringService.Application.Abstractions;
using OrienteeringService.Domain.Maps;
using OrienteeringService.Infrastructure.Persistence;

namespace OrienteeringService.Infrastructure.Repositories
{
    public class MapRepository(AppDbContext appDbContext) : IMapRepository
    {
        public Task<SportMap?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return appDbContext.Maps
                .AsNoTracking()
                .FirstOrDefaultAsync(map => map.Id == id, cancellationToken);
        }

        public Task<SportMap?> GetByIdAndOwnerIdAsync(
            Guid id,
            Guid ownerId,
            CancellationToken cancellationToken)
        {
            return appDbContext.Maps.FirstOrDefaultAsync(
                map => map.Id == id && map.OwnerId == ownerId,
                cancellationToken);
        }

        public async Task<IReadOnlyCollection<SportMap>> GetByOwnerIdAsync(
            Guid ownerId,
            CancellationToken cancellationToken)
        {
            return await appDbContext.Maps
                .AsNoTracking()
                .Where(map => map.OwnerId == ownerId)
                .OrderByDescending(map => map.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<SportMap>> GetPublicMapsByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            return await appDbContext.Maps
                .AsNoTracking()
                .Where(map => map.OwnerId == userId && map.Visibility == MapVisibility.Public)
                .OrderByDescending(map => map.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(SportMap sportMap, CancellationToken cancellationToken)
        {
            await appDbContext.Maps.AddAsync(sportMap, cancellationToken);
            await appDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(SportMap sportMap, CancellationToken cancellationToken)
        {
            appDbContext.Maps.Update(sportMap);
            await appDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(SportMap sportMap, CancellationToken cancellationToken)
        {
            appDbContext.Maps.Remove(sportMap);
            await appDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
