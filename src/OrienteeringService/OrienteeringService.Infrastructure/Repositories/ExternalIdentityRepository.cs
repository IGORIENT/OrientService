using Microsoft.EntityFrameworkCore;
using Npgsql;
using OrienteeringService.Application.Abstractions;
using OrienteeringService.Domain.Users;
using OrienteeringService.Infrastructure.Persistence;

namespace OrienteeringService.Infrastructure.Repositories
{
    public class ExternalIdentityRepository(AppDbContext appDbContext) : IExternalIdentityRepository
    {
        public Task<User?> GetUserByIssuerAndSubjectAsync(
            string issuer,
            string subject,
            CancellationToken cancellationToken)
        {
            return appDbContext.ExternalIdentities
                .Where(identity => identity.Issuer == issuer && identity.Subject == subject)
                .Join(
                    appDbContext.Users,
                    identity => identity.UserId,
                    user => user.Id,
                    (_, user) => user)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<User> AddAsync(
            User user,
            ExternalIdentity externalIdentity,
            CancellationToken cancellationToken)
        {
            await appDbContext.Users.AddAsync(user, cancellationToken);
            await appDbContext.ExternalIdentities.AddAsync(externalIdentity, cancellationToken);

            try
            {
                // SaveChanges wraps both inserts in one transaction.
                await appDbContext.SaveChangesAsync(cancellationToken);
                return user;
            }
            catch (DbUpdateException exception) when (IsExternalIdentityConflict(exception))
            {
                // Two first requests may provision the same Keycloak account concurrently.
                // The unique index selects the winner; this request reads that user back.
                appDbContext.ChangeTracker.Clear();

                var existingUser = await GetUserByIssuerAndSubjectAsync(
                    externalIdentity.Issuer,
                    externalIdentity.Subject,
                    cancellationToken);

                if (existingUser is null)
                {
                    throw;
                }

                return existingUser;
            }
        }

        private static bool IsExternalIdentityConflict(DbUpdateException exception)
        {
            return exception.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation,
                ConstraintName: AppDbContext.ExternalIdentityIndexName,
            };
        }
    }
}
