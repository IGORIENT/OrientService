using System;
using System.Collections.Generic;
using System.Text;

namespace OrienteeringService.Domain.Maps
{
    public class SportMap
    {
        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }

        public required string Title { get; set; }

        public string? Description { get; set; }

        public required string ImagePath { get; set; }

        public required MapBounds Bounds { get; set; }

        public required MapSportType SportType { get; set; }

        public MapVisibility Visibility { get; set; } = MapVisibility.Private;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
