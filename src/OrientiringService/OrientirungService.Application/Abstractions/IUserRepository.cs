using OrientiringService.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrientiringService.Application.Abstractions
{

    // Репозиторий — это доступ к доменной модели
    // Он должен:    //сохранять User    //получать User    //искать User
    //То есть его задача — работать с Domain, а не с HTTP-моделями.
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<User?> GetByLoginAsync(string login, CancellationToken cancellationToken);

        Task<IReadOnlyCollection<User>> SearchAsync(string query, CancellationToken cancellationToken);

        Task AddAsync(User user, CancellationToken cancellationToken);
    }
}
