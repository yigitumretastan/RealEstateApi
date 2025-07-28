namespace RealEstateApi.DTOs
{
    public class UpdateListingDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? City { get; set; }
        public string? District { get; set; } // İlçe alanı eklendi
        public string? Street { get; set; } // Sokak
        public string? ApartmentNumber { get; set; } // Daire numarası
        public string? RoomType { get; set; } // Oda tipi
        public decimal? Price { get; set; }
    }
}