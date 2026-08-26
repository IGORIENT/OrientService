using OrienteeringService.Application.Abstractions;
using OrienteeringService.Domain.Maps;

namespace OrienteeringService.Application.Maps
{
    public class MapService(IMapRepository mapRepository, ICurrentUser currentUser)
    {
        public async Task<Guid> CreateAsync(
            CreateMapCommand command,
            CancellationToken cancellationToken)
        {
            Validate(
                command.Title,
                command.Description,
                command.ImagePath,
                command.North,
                command.South,
                command.West,
                command.East,
                command.SportType,
                command.Visibility);

            var now = DateTime.UtcNow;
            var map = new SportMap
            {
                Id = Guid.NewGuid(),
                OwnerId = currentUser.UserId,
                Title = command.Title.Trim(),
                Description = command.Description?.Trim(),
                ImagePath = command.ImagePath.Trim(),
                SportType = command.SportType,
                Visibility = command.Visibility,
                Bounds = new MapBounds
                {
                    North = command.North,
                    South = command.South,
                    West = command.West,
                    East = command.East,
                },
                CreatedAt = now,
                UpdatedAt = now,
            };

            await mapRepository.AddAsync(map, cancellationToken);

            return map.Id;
        }

        public async Task<SportMap?> GetPublicByIdAsync(Guid mapId, CancellationToken cancellationToken)
        {
            var map = await mapRepository.GetByIdAsync(mapId, cancellationToken);

            return map?.Visibility == MapVisibility.Public ? map : null;
        }

        public Task<SportMap?> GetOwnedByIdAsync(Guid mapId, CancellationToken cancellationToken)
        {
            return mapRepository.GetByIdAndOwnerIdAsync(mapId, currentUser.UserId, cancellationToken);
        }

        public Task<IReadOnlyCollection<SportMap>> GetMyMapsAsync(CancellationToken cancellationToken)
        {
            return mapRepository.GetByOwnerIdAsync(currentUser.UserId, cancellationToken);
        }

        public Task<IReadOnlyCollection<SportMap>> GetPublicMapsByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            return mapRepository.GetPublicMapsByUserIdAsync(userId, cancellationToken);
        }

        public async Task<bool> UpdateOwnedAsync(
            Guid mapId,
            UpdateMapCommand command,
            CancellationToken cancellationToken)
        {
            Validate(
                command.Title,
                command.Description,
                command.ImagePath,
                command.North,
                command.South,
                command.West,
                command.East,
                command.SportType,
                command.Visibility);

            var map = await mapRepository.GetByIdAndOwnerIdAsync(
                mapId,
                currentUser.UserId,
                cancellationToken);

            if (map is null)
            {
                return false;
            }

            map.Title = command.Title.Trim();
            map.Description = command.Description?.Trim();
            map.ImagePath = command.ImagePath.Trim();
            map.SportType = command.SportType;
            map.Visibility = command.Visibility;
            map.Bounds.North = command.North;
            map.Bounds.South = command.South;
            map.Bounds.West = command.West;
            map.Bounds.East = command.East;
            map.UpdatedAt = DateTime.UtcNow;

            await mapRepository.UpdateAsync(map, cancellationToken);
            return true;
        }

        public async Task<bool> DeleteOwnedAsync(Guid mapId, CancellationToken cancellationToken)
        {
            var map = await mapRepository.GetByIdAndOwnerIdAsync(
                mapId,
                currentUser.UserId,
                cancellationToken);

            if (map is null)
            {
                return false;
            }

            await mapRepository.DeleteAsync(map, cancellationToken);
            return true;
        }

        private static void Validate(
            string title,
            string? description,
            string imagePath,
            double north,
            double south,
            double west,
            double east,
            MapSportType sportType,
            MapVisibility visibility)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title);
            ArgumentException.ThrowIfNullOrWhiteSpace(imagePath);

            if (title.Trim().Length > 300)
            {
                throw new ArgumentException("Title must not exceed 300 characters.", nameof(title));
            }

            if (imagePath.Trim().Length > 1000)
            {
                throw new ArgumentException("ImagePath must not exceed 1000 characters.", nameof(imagePath));
            }

            if (description?.Trim().Length > 4000)
            {
                throw new ArgumentException("Description must not exceed 4000 characters.", nameof(description));
            }

            if (!double.IsFinite(north)
                || !double.IsFinite(south)
                || !double.IsFinite(west)
                || !double.IsFinite(east))
            {
                throw new ArgumentException("Map bounds must contain finite numbers.");
            }

            if (north is < -90 or > 90 || south is < -90 or > 90)
            {
                throw new ArgumentException("North and South must be between -90 and 90 degrees.");
            }

            if (west is < -180 or > 180 || east is < -180 or > 180)
            {
                throw new ArgumentException("West and East must be between -180 and 180 degrees.");
            }

            if (north <= south)
            {
                throw new ArgumentException("North must be greater than South.");
            }

            if (east <= west)
            {
                throw new ArgumentException("East must be greater than West.");
            }

            if (!Enum.IsDefined(sportType))
            {
                throw new ArgumentException("Unknown map sport type.", nameof(sportType));
            }

            if (!Enum.IsDefined(visibility))
            {
                throw new ArgumentException("Unknown map visibility.", nameof(visibility));
            }
        }
    }
}
