using System.Text.Json.Serialization;

namespace RealEstateApi.Models
{
    public class Listing
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string District { get; set; } = string.Empty;

        public string RoomType { get; set; } = string.Empty;

        public string Street { get; set; } = string.Empty;

        public string? ApartmentNumber { get; set; }

        public decimal Price { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }

        [JsonIgnore]
        public User User { get; set; } = null!;
    }
}