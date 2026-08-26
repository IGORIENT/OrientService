using System.Security.Claims;
using OrienteeringService.Application.Auth;

namespace OrienteeringService.Web.Authentication
{
    internal sealed class LocalUserProvisioningMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(
            HttpContext context,
            AuthService authService)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                await AddLocalUserAsync(context, authService);
            }

            await next(context);
        }

        private static async Task AddLocalUserAsync(HttpContext context, AuthService authService)
        {
            if (context.User.HasClaim(claim => claim.Type == LocalUserClaims.UserId))
            {
                return;
            }

            var issuer = context.User.FindFirstValue("iss")
                         ?? throw new InvalidOperationException("The access token has no issuer claim.");
            var subject = context.User.FindFirstValue("sub")
                          ?? throw new InvalidOperationException("The access token has no subject claim.");

            var user = await authService.GetOrCreateUserAsync(
                issuer,
                subject,
                context.User.FindFirstValue("name"),
                context.User.FindFirstValue("preferred_username"),
                GetVerifiedEmail(context.User),
                context.RequestAborted);

            if (context.User.Identity is not ClaimsIdentity identity)
            {
                throw new InvalidOperationException("The authenticated identity cannot contain claims.");
            }

            identity.AddClaim(new Claim(LocalUserClaims.UserId, user.Id.ToString()));
        }

        private static string? GetVerifiedEmail(ClaimsPrincipal principal)
        {
            var emailVerified = principal.FindFirstValue("email_verified");
            return string.Equals(emailVerified, bool.TrueString, StringComparison.OrdinalIgnoreCase)
                ? principal.FindFirstValue("email")
                : null;
        }
    }
}
