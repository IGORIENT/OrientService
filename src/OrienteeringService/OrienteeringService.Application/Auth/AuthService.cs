using OrienteeringService.Application.Abstractions;
using OrienteeringService.Domain.Users;

namespace OrienteeringService.Application.Auth
{
    public class AuthService(IExternalIdentityRepository externalIdentityRepository)
    {
        public async Task<User> GetOrCreateUserAsync(
            string issuer,
            string subject,
            string? displayName,
            string? login,
            string? email,
            CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(issuer);
            ArgumentException.ThrowIfNullOrWhiteSpace(subject);

            var existingUser = await externalIdentityRepository.GetUserByIssuerAndSubjectAsync(
                issuer,
                subject,
                cancellationToken);

            if (existingUser is not null)
            {
                return existingUser;
            }

            var userId = Guid.NewGuid();
            var normalizedLogin = CreateLogin(login, userId);

            var user = new User
            {
                Id = userId,
                DisplayName = CreateDisplayName(displayName, normalizedLogin),
                Login = normalizedLogin,
                CreatedAt = DateTime.UtcNow,
            };

            var externalIdentity = new ExternalIdentity
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Issuer = issuer,
                Subject = subject,
                Email = NormalizeEmail(email),
                LinkedAt = DateTime.UtcNow,
            };

            return await externalIdentityRepository.AddAsync(user, externalIdentity, cancellationToken);
        }

        private static string CreateLogin(string? login, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                return $"user-{userId:N}";
            }

            var value = login.Trim();
            return value[..Math.Min(value.Length, 100)];
        }

        private static string CreateDisplayName(string? displayName, string login)
        {
            var value = string.IsNullOrWhiteSpace(displayName) ? login : displayName.Trim();
            return value[..Math.Min(value.Length, 200)];
        }

        private static string? NormalizeEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            var value = email.Trim();
            return value.Length <= 320 ? value : null;
        }
    }
}
