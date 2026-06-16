using Microsoft.EntityFrameworkCore;
using OrientiringService.Application.Abstractions;
using OrientiringService.Domain.Maps;
using OrientiringService.Infrastructure.Persistence;

namespace OrientiringService.Infrastructure.Repositories
{
    public class MapRepository : IMapRepository
    {
        private readonly AppDbContext appDbContext;

        public MapRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<SportMap?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await appDbContext.Maps.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IReadOnlyCollection<SportMap>> GetPublicMapsByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await appDbContext.Maps
                .Where(x => x.OwnerId == userId && x.Visability == MapVisability.Public)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(SportMap sportMap, CancellationToken cancellationToken)
        {
            await appDbContext.Maps.AddAsync(sportMap);
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
