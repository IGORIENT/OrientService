using OrientiringService.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrientiringService.Application.Abstractions
{
    public interface IExternalIdentityRepository
    {
        Task<ExternalIdentity?> GetByproviderAndSubjectAsync(
            string provider,
            string subject,
            CancellationToken cancellationToken);

        Task AddAsync(ExternalIdentity externalIdentity, CancellationToken cancellationToken);
    }
}
