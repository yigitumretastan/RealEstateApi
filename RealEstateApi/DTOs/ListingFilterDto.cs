namespace Real_Estate_Api.DTOs
{
    public class ListingFilterDto
    {
        public string? City { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? RoomCount { get; set; }
    }
}
