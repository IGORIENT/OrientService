using Microsoft.EntityFrameworkCore;
using OrienteeringService.Application.Abstractions;
using OrienteeringService.Domain.Users;
using OrienteeringService.Infrastructure.Persistence;

namespace OrienteeringService.Infrastructure.Repositories
{
    public class UserRepository(AppDbContext appDbContext) : IUserRepository
    {
        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return appDbContext.Users.FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
        }

        public Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken)
        {
            return appDbContext.Users.FirstOrDefaultAsync(user => user.Login == login, cancellationToken);
        }

        public async Task<IReadOnlyCollection<User>> SearchAsync(
            string query,
            CancellationToken cancellationToken)
        {
            query = query.Trim();

            return await appDbContext.Users
                .Where(user => user.DisplayName.Contains(query) || user.Login.Contains(query))
                .OrderBy(user => user.DisplayName)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken)
        {
            await appDbContext.Users.AddAsync(user, cancellationToken);
            await appDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
