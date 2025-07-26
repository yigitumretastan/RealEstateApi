namespace Real_Estate_Api.DTOs
{
    public class UpdateListingDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? City { get; set; }
        public int? RoomCount { get; set; }
        public decimal? Price { get; set; }
    }
}
