namespace RealEstateApi.DTOs
{
    public class ListingFilterDto
    {

        public string? SearchTerm { get; set; }

        public string? City { get; set; }
        public string? District { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public string? RoomType { get; set; }


        public string? Street { get; set; }


        public string? SortBy { get; set; } 
        public string? SortOrder { get; set; } = "asc";

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}