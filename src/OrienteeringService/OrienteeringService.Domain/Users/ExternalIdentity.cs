namespace OrienteeringService.Domain.Users
{
    public class ExternalIdentity
    {
        public Guid Id { get; init; }

        public Guid UserId { get; init; }

        public required string Issuer { get; init; }

        public required string Subject { get; init; }

        public string? Email { get; init; }

        public DateTime LinkedAt { get; init; }
    }
}
