using Microsoft.EntityFrameworkCore;
using OrientiringService.Domain.Maps;
using OrientiringService.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrientiringService.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();

        public DbSet<SportMap> Maps => Set<SportMap>();

        public DbSet<ExternalIdentity> ExternalIdentities => Set<ExternalIdentity>();
    }
}
