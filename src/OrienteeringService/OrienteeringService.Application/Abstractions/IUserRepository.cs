using OrienteeringService.Domain.Users;

namespace OrienteeringService.Application.Abstractions
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken);

        Task<IReadOnlyCollection<User>> SearchAsync(string query, CancellationToken cancellationToken);

        Task AddAsync(User user, CancellationToken cancellationToken);
    }
}
