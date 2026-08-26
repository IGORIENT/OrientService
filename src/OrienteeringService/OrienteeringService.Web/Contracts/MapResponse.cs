using OrienteeringService.Domain.Maps;

namespace OrienteeringService.Web.Contracts
{
    public sealed record MapResponse(
        Guid Id,
        Guid OwnerId,
        string Title,
        string? Description,
        string ImagePath,
        double North,
        double South,
        double West,
        double East,
        MapSportType SportType,
        MapVisibility Visibility,
        DateTime CreatedAt,
        DateTime UpdatedAt);
}
