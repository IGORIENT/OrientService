using System.Security.Claims;
using OrienteeringService.Application.Abstractions;

namespace OrienteeringService.Web.Authentication
{
    internal sealed class HttpCurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
    {
        public Guid UserId
        {
            get
            {
                var value = httpContextAccessor.HttpContext?.User.FindFirstValue(LocalUserClaims.UserId);

                if (!Guid.TryParse(value, out var userId))
                {
                    throw new InvalidOperationException("The current request has no local user.");
                }

                return userId;
            }
        }
    }
}
