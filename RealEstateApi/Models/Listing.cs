namespace RealEstateApi.Models
{
    public class Listing
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string District { get; set; } = string.Empty; // İlçe alanı eklendi

        public string RoomType { get; set; } = string.Empty; // "1+1", "2+1", "3+1", "4+1", "Dublex" vb.

        public string Street { get; set; } = string.Empty; // Sokak bilgisi

        public string? ApartmentNumber { get; set; } // Daire numarası (opsiyonel)

        public decimal Price { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign Key
        public int UserId { get; set; }

        // Navigation property
        public User User { get; set; } = null!;
    }
}