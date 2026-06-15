using OrientiringService.Application.Abstractions;
using OrientiringService.Domain.Users;
using OrientiringService.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

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
            return appDbContext.Users.First
        }


    }
}
