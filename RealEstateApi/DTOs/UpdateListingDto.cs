namespace RealEstateApi.DTOs
{
    public class UpdateListingDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Street { get; set; }
        public string? ApartmentNumber { get; set; }
        public string? RoomType { get; set; }
        public decimal? Price { get; set; }
    }
}