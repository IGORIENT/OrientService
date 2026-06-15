using OrientiringService.Application.Abstractions;
using OrientiringService.Domain.Maps;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrientiringService.Application.Maps
{
    public class MapService
    {
        private readonly IMapRepository mapRepository;

        public MapService(IMapRepository mapRepository)
        {
            this.mapRepository = mapRepository;
        }

        public async Task<Guid> CreateAsync(
            Guid ownerId,
            string title,
            string? description,
            string imagePath,
            double north,
            double south,
            double west,
            double east,
            MapSportType sportType,
            MapVisability visability,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required.");

            if (string.IsNullOrWhiteSpace(imagePath))
                throw new ArgumentException("ImagePath is required.");

            if (north <= south)
                throw new ArgumentException("North must be greater than South.");

            if (east <= west)
                throw new ArgumentException("East must be greater than West.");


            var map = new SportMap
            {
                Id = Guid.NewGuid(),
                OwnerId = ownerId,
                Title = title,
                Description = description,
                ImagePath = imagePath,
                SportType = sportType,
                Visability = visability,
                Bounds = new MapBounds
                {
                    North = north,
                    South = south,
                    West = west,
                    East = east
                },
                CreatedAt = DateTime.UtcNow
            };

             await mapRepository.AddAsync(map, cancellationToken);

            return map.Id;
        }

        public async Task<SportMap?> GetPublicByIdAsync(Guid mapId,  CancellationToken cancellationToken)
        {
            var map = await mapRepository.GetByIdAsync(mapId, cancellationToken);

            if (map is null)
            {
                return null;
            }

            if (map.Visability != MapVisability.Public)
            {
                return null;
            }

            return map;
        }

        public async Task<IReadOnlyCollection<SportMap>> GetPublicMapsByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var maps = await mapRepository.GetPublicMapsByUserIdAsync(userId, cancellationToken);

            return maps;
        }
    }
}
