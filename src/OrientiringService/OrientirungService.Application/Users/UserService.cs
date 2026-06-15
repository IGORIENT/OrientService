using System;
using System.Collections.Generic;
using System.Text;
using OrientiringService.Application.Abstractions;
using OrientiringService.Domain.Users;

namespace OrientiringService.Application.Users
{
    public class UserService
    {
        private readonly IUserRepository userRepository;

        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository; 
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByIdAsync(id, cancellationToken);
            return user;
        }

        public async Task<IReadOnlyCollection<User>> SearchAsync(string query, CancellationToken cancellationToken)
        {
            var users = await userRepository.SearchAsync(query, cancellationToken);
            return users;
        }
    }
}
