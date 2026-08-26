using OrienteeringService.Application.Abstractions;
using OrienteeringService.Domain.Users;

namespace OrienteeringService.Application.Users
{
    public class UserService(IUserRepository userRepository, ICurrentUser currentUser)
    {
        public Task<User?> GetCurrentAsync(CancellationToken cancellationToken)
        {
            return userRepository.GetByIdAsync(currentUser.UserId, cancellationToken);
        }

        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return userRepository.GetByIdAsync(id, cancellationToken);
        }

        public Task<IReadOnlyCollection<User>> SearchAsync(string query, CancellationToken cancellationToken)
        {
            return userRepository.SearchAsync(query, cancellationToken);
        }
    }
}
