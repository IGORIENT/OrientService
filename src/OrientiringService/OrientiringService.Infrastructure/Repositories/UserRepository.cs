using Microsoft.EntityFrameworkCore;
using OrientiringService.Application.Abstractions;
using OrientiringService.Domain.Users;
using OrientiringService.Infrastructure.Persistence;


namespace OrientiringService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext appDbContext;

        public UserRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return appDbContext.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken)
        {
            return appDbContext.Users.FirstOrDefaultAsync(x => x.Login == login, cancellationToken);
        }

        public async Task<IReadOnlyCollection<User>> SearchAsync(string query, CancellationToken cancellationToken)
        {
            query = query.Trim();
            return await appDbContext.Users
                .Where(x => 
                x.DisplayName.Contains(query) || 
                (x.Login != null && x.Login.Contains(query)))
                .OrderBy(x => x.DisplayName)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken)
        { 
            await appDbContext.Users.AddAsync(user, cancellationToken);
            await appDbContext.SaveChangesAsync(cancellationToken);
        }

        ////public async void UpdateAsync(User user, CancellationToken cancellationToken)
        ////{
        ////    appDbContext.Update(user);
        ////    await appDbContext.SaveChangesAsync(cancellationToken);
        ////}


    }
}
