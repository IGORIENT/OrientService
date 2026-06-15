using OrientiringService.Application.Abstractions;
using OrientiringService.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrientiringService.Application.Auth
{
    public class AuthService
    {
        private readonly IUserRepository userRepository;
        private readonly IExternalIdentityRepository externalIdentityRepository;

        public AuthService(IUserRepository userRepository, IExternalIdentityRepository externalIdentityRepository)
        {
            this.userRepository = userRepository;
            this.externalIdentityRepository = externalIdentityRepository;
        }

        public async Task<User> GetOrCreateUserAsync(
            string provider,
            string subject,
            string displayName,
            string? login,
            string? email,
            CancellationToken cancellationToken)
        {
            var externalIdentity = await externalIdentityRepository.GetByproviderAndSubjectAsync(provider, subject, cancellationToken);

            if (externalIdentity is not null)
            {
                var existingUser = await userRepository.GetByIdAsync(externalIdentity.UserId, cancellationToken);

                if (existingUser is null)
                    throw new InvalidOperationException("User for external identity was not found.");

                return existingUser;
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                DisplayName = displayName,
                Login = login,
                CreatedAt = DateTime.UtcNow,
            };

            var newExternalIdentity = new ExternalIdentity
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Provider = provider,
                Subject = subject,
                Email = email,
                LinkedAt = DateTime.UtcNow,
            };

            await userRepository.AddAsync(user, cancellationToken);
            await externalIdentityRepository.AddAsync(newExternalIdentity, cancellationToken);

            return user;
        }
    }
}
