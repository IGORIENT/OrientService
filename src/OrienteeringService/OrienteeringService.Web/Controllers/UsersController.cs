using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrienteeringService.Application.Users;
using OrienteeringService.Web.Contracts;

namespace OrienteeringService.Web.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/users")]
    public class UsersController(UserService userService) : ControllerBase
    {
        [HttpGet("me")]
        [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserResponse>> GetMe(CancellationToken cancellationToken)
        {
            var user = await userService.GetCurrentAsync(cancellationToken);

            if (user is null)
            {
                return NotFound();
            }

            return Ok(new UserResponse(
                user.Id,
                user.DisplayName,
                user.Login,
                user.About,
                user.CreatedAt));
        }
    }
}
