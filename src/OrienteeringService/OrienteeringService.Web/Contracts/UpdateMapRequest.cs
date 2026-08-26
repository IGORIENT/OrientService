using OrienteeringService.Domain.Maps;

namespace OrienteeringService.Web.Contracts
{
    public sealed record UpdateMapRequest(
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
