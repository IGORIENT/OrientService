using OrienteeringService.Application.Abstractions;
using OrienteeringService.Application.Maps;
using OrienteeringService.Domain.Maps;

namespace OrienteeringService.Application.Tests.Maps
{
    public class MapServiceTests
    {
        [Fact]
        public async Task CreateAsyncUsesAuthenticatedUserAsOwner()
        {
            var userId = Guid.NewGuid();
            var repository = new FakeMapRepository();
            var service = new MapService(repository, new FakeCurrentUser(userId));

            var mapId = await service.CreateAsync(CreateCommand(), CancellationToken.None);

            Assert.NotNull(repository.AddedMap);
            Assert.Equal(mapId, repository.AddedMap.Id);
            Assert.Equal(userId, repository.AddedMap.OwnerId);
        }

        [Fact]
        public async Task UpdateOwnedAsyncUpdatesOwnersMap()
        {
            var userId = Guid.NewGuid();
            var map = CreateMap(userId);
            var repository = new FakeMapRepository { ExistingMap = map };
            var service = new MapService(repository, new FakeCurrentUser(userId));

            var updated = await service.UpdateOwnedAsync(
                map.Id,
                new UpdateMapCommand(
                    "Updated map",
                    "Updated description",
                    "maps/updated.png",
                    61,
                    51,
                    31,
                    41,
                    MapSportType.CitySprintRun,
                    MapVisibility.Public),
                CancellationToken.None);

            Assert.True(updated);
            Assert.Same(map, repository.UpdatedMap);
            Assert.Equal("Updated map", map.Title);
            Assert.Equal(MapVisibility.Public, map.Visibility);
        }

        [Fact]
        public async Task UpdateOwnedAsyncDoesNotUpdateAnotherUsersMap()
        {
            var map = CreateMap(Guid.NewGuid());
            var repository = new FakeMapRepository { ExistingMap = map };
            var service = new MapService(repository, new FakeCurrentUser(Guid.NewGuid()));

            var updated = await service.UpdateOwnedAsync(
                map.Id,
                UpdateCommand(),
                CancellationToken.None);

            Assert.False(updated);
            Assert.Null(repository.UpdatedMap);
        }

        [Fact]
        public async Task DeleteOwnedAsyncDoesNotDeleteAnotherUsersMap()
        {
            var map = CreateMap(Guid.NewGuid());
            var repository = new FakeMapRepository { ExistingMap = map };
            var service = new MapService(repository, new FakeCurrentUser(Guid.NewGuid()));

            var deleted = await service.DeleteOwnedAsync(map.Id, CancellationToken.None);

            Assert.False(deleted);
            Assert.Null(repository.DeletedMap);
        }

        [Fact]
        public async Task GetPublicByIdAsyncHidesPrivateMap()
        {
            var map = CreateMap(Guid.NewGuid());
            var repository = new FakeMapRepository { ExistingMap = map };
            var service = new MapService(repository, new FakeCurrentUser(Guid.NewGuid()));

            var result = await service.GetPublicByIdAsync(map.Id, CancellationToken.None);

            Assert.Null(result);
        }

        private static CreateMapCommand CreateCommand()
        {
            return new CreateMapCommand(
                "Training map",
                null,
                "maps/training.png",
                60,
                50,
                30,
                40,
                MapSportType.ForestRun,
                MapVisibility.Private);
        }

        private static UpdateMapCommand UpdateCommand()
        {
            return new UpdateMapCommand(
                "Training map",
                null,
                "maps/training.png",
                60,
                50,
                30,
                40,
                MapSportType.ForestRun,
                MapVisibility.Private);
        }

        private static SportMap CreateMap(Guid ownerId)
        {
            return new SportMap
            {
                Id = Guid.NewGuid(),
                OwnerId = ownerId,
                Title = "Private map",
                ImagePath = "maps/private.png",
                Bounds = new MapBounds
                {
                    North = 60,
                    South = 50,
                    West = 30,
                    East = 40,
                },
                SportType = MapSportType.ForestRun,
                Visibility = MapVisibility.Private,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
        }

        private sealed class FakeCurrentUser(Guid userId) : ICurrentUser
        {
            public Guid UserId { get; } = userId;
        }

        private sealed class FakeMapRepository : IMapRepository
        {
            public SportMap? ExistingMap { get; init; }

            public SportMap? AddedMap { get; private set; }

            public SportMap? UpdatedMap { get; private set; }

            public SportMap? DeletedMap { get; private set; }

            public Task<SportMap?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
            {
                return Task.FromResult(ExistingMap?.Id == id ? ExistingMap : null);
            }

            public Task<SportMap?> GetByIdAndOwnerIdAsync(
                Guid id,
                Guid ownerId,
                CancellationToken cancellationToken)
            {
                var result = ExistingMap?.Id == id && ExistingMap.OwnerId == ownerId
                    ? ExistingMap
                    : null;

                return Task.FromResult(result);
            }

            public Task<IReadOnlyCollection<SportMap>> GetByOwnerIdAsync(
                Guid ownerId,
                CancellationToken cancellationToken)
            {
                IReadOnlyCollection<SportMap> result = ExistingMap?.OwnerId == ownerId
                    ? [ExistingMap]
                    : [];

                return Task.FromResult(result);
            }

            public Task<IReadOnlyCollection<SportMap>> GetPublicMapsByUserIdAsync(
                Guid userId,
                CancellationToken cancellationToken)
            {
                IReadOnlyCollection<SportMap> result = ExistingMap?.OwnerId == userId
                                                       && ExistingMap.Visibility == MapVisibility.Public
                    ? [ExistingMap]
                    : [];

                return Task.FromResult(result);
            }

            public Task AddAsync(SportMap sportMap, CancellationToken cancellationToken)
            {
                AddedMap = sportMap;
                return Task.CompletedTask;
            }

            public Task UpdateAsync(SportMap sportMap, CancellationToken cancellationToken)
            {
                UpdatedMap = sportMap;
                return Task.CompletedTask;
            }

            public Task DeleteAsync(SportMap sportMap, CancellationToken cancellationToken)
            {
                DeletedMap = sportMap;
                return Task.CompletedTask;
            }
        }
    }
}
