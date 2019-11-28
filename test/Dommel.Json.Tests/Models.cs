using System;

namespace Dommel.Json.Tests
{
    public class Lead
    {
        public int Id { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public string Email { get; set; }

        [JsonData]
        public LeadData Data { get; set; }
    }

    public class LeadData
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public DateTime Birthdate { get; set; }
    }
}
