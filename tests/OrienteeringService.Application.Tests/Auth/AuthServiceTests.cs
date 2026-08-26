using OrienteeringService.Application.Abstractions;
using OrienteeringService.Application.Auth;
using OrienteeringService.Domain.Users;

namespace OrienteeringService.Application.Tests.Auth
{
    public class AuthServiceTests
    {
        [Fact]
        public async Task GetOrCreateUserAsyncReturnsExistingUser()
        {
            var existingUser = CreateUser();
            var repository = new FakeExternalIdentityRepository
            {
                ExistingUser = existingUser,
            };
            var service = new AuthService(repository);

            var result = await service.GetOrCreateUserAsync(
                "https://auth.example/realms/main",
                "keycloak-subject",
                "Ignored name",
                "ignored-login",
                null,
                CancellationToken.None);

            Assert.Same(existingUser, result);
            Assert.Null(repository.AddedUser);
            Assert.Null(repository.AddedIdentity);
        }

        [Fact]
        public async Task GetOrCreateUserAsyncCreatesLinkedUser()
        {
            const string issuer = "https://auth.example/realms/main";
            const string subject = "keycloak-subject";
            var repository = new FakeExternalIdentityRepository();
            var service = new AuthService(repository);

            var result = await service.GetOrCreateUserAsync(
                issuer,
                subject,
                "Alice Runner",
                "alice",
                "alice@example.com",
                CancellationToken.None);

            Assert.Same(repository.AddedUser, result);
            Assert.NotNull(repository.AddedIdentity);
            Assert.Equal(result.Id, repository.AddedIdentity.UserId);
            Assert.Equal(issuer, repository.AddedIdentity.Issuer);
            Assert.Equal(subject, repository.AddedIdentity.Subject);
            Assert.Equal("alice", result.Login);
            Assert.Equal("Alice Runner", result.DisplayName);
        }

        private static User CreateUser()
        {
            return new User
            {
                Id = Guid.NewGuid(),
                DisplayName = "Existing user",
                Login = "existing",
                CreatedAt = DateTime.UtcNow,
            };
        }

        private sealed class FakeExternalIdentityRepository : IExternalIdentityRepository
        {
            public User? ExistingUser { get; init; }

            public User? AddedUser { get; private set; }

            public ExternalIdentity? AddedIdentity { get; private set; }

            public Task<User?> GetUserByIssuerAndSubjectAsync(
                string issuer,
                string subject,
                CancellationToken cancellationToken)
            {
                return Task.FromResult(ExistingUser);
            }

            public Task<User> AddAsync(
                User user,
                ExternalIdentity externalIdentity,
                CancellationToken cancellationToken)
            {
                AddedUser = user;
                AddedIdentity = externalIdentity;
                return Task.FromResult(user);
            }
        }
    }
}
