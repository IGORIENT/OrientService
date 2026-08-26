using OrienteeringService.Domain.Users;

namespace OrienteeringService.Application.Abstractions
{
    public interface IExternalIdentityRepository
    {
        Task<User?> GetUserByIssuerAndSubjectAsync(
            string issuer,
            string subject,
            CancellationToken cancellationToken);

        Task<User> AddAsync(
            User user,
            ExternalIdentity externalIdentity,
            CancellationToken cancellationToken);
    }
}
