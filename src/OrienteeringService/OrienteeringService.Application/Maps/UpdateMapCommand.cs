using OrienteeringService.Domain.Maps;

namespace OrienteeringService.Application.Maps
{
    public sealed record UpdateMapCommand(
        string Title,
        string? Description,
        string ImagePath,
        double North,
        double South,
        double West,
        double East,
        MapSportType SportType,
        MapVisibility Visibility);
}
