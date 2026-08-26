namespace OrienteeringService.Web.Contracts
{
    public sealed record UserResponse(
        Guid Id,
        string DisplayName,
        string Login,
        string? About,
        DateTime CreatedAt);
}
