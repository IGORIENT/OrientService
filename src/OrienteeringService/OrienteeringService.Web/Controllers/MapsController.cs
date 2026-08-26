using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrienteeringService.Application.Maps;
using OrienteeringService.Domain.Maps;
using OrienteeringService.Web.Contracts;

namespace OrienteeringService.Web.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v1/maps")]
    public class MapsController(MapService mapService) : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType<CreateMapResponse>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<CreateMapResponse>> Create(
            CreateMapRequest request,
            CancellationToken cancellationToken)
        {
            var mapId = await mapService.CreateAsync(
                new CreateMapCommand(
                    request.Title,
                    request.Description,
                    request.ImagePath,
                    request.North,
                    request.South,
                    request.West,
                    request.East,
                    request.SportType,
                    request.Visibility),
                cancellationToken);

            return CreatedAtAction(nameof(GetMineById), new { mapId }, new CreateMapResponse(mapId));
        }

        [HttpGet("{mapId:guid}")]
        [AllowAnonymous]
        [ProducesResponseType<MapResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MapResponse>> GetPublicById(
            Guid mapId,
            CancellationToken cancellationToken)
        {
            var map = await mapService.GetPublicByIdAsync(mapId, cancellationToken);
            return map is null ? NotFound() : Ok(ToResponse(map));
        }

        [HttpGet("users/{userId:guid}")]
        [AllowAnonymous]
        [ProducesResponseType<IReadOnlyCollection<MapResponse>>(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyCollection<MapResponse>>> GetPublicByUserId(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var maps = await mapService.GetPublicMapsByUserIdAsync(userId, cancellationToken);
            return Ok(maps.Select(ToResponse).ToArray());
        }

        [HttpGet("mine")]
        [ProducesResponseType<IReadOnlyCollection<MapResponse>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IReadOnlyCollection<MapResponse>>> GetMine(
            CancellationToken cancellationToken)
        {
            var maps = await mapService.GetMyMapsAsync(cancellationToken);
            return Ok(maps.Select(ToResponse).ToArray());
        }

        [HttpGet("mine/{mapId:guid}")]
        [ProducesResponseType<MapResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MapResponse>> GetMineById(
            Guid mapId,
            CancellationToken cancellationToken)
        {
            var map = await mapService.GetOwnedByIdAsync(mapId, cancellationToken);
            return map is null ? NotFound() : Ok(ToResponse(map));
        }

        [HttpPut("{mapId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(
            Guid mapId,
            UpdateMapRequest request,
            CancellationToken cancellationToken)
        {
            var updated = await mapService.UpdateOwnedAsync(
                mapId,
                new UpdateMapCommand(
                    request.Title,
                    request.Description,
                    request.ImagePath,
                    request.North,
                    request.South,
                    request.West,
                    request.East,
                    request.SportType,
                    request.Visibility),
                cancellationToken);

            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{mapId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid mapId, CancellationToken cancellationToken)
        {
            var deleted = await mapService.DeleteOwnedAsync(mapId, cancellationToken);
            return deleted ? NoContent() : NotFound();
        }

        private static MapResponse ToResponse(SportMap map)
        {
            return new MapResponse(
                map.Id,
                map.OwnerId,
                map.Title,
                map.Description,
                map.ImagePath,
                map.Bounds.North,
                map.Bounds.South,
                map.Bounds.West,
                map.Bounds.East,
                map.SportType,
                map.Visibility,
                map.CreatedAt,
                map.UpdatedAt);
        }
    }
}
