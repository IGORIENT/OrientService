using System;
using System.Collections.Generic;
using System.Text;

namespace OrienteeringService.Domain.Users
{
    public class User
    {
        public Guid Id { get; set; }

        public required string DisplayName { get; set; }

        public required string Login {  get; set; }

        public string? About {  get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
