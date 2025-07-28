namespace Real_Estate_Api.DTOs
{
    public class ListingFilterDto
    {
        // Metin tabanlı arama (başlık ve açıklamada)
        public string? SearchTerm { get; set; }

        // Konum filtreleri
        public string? City { get; set; }
        public string? District { get; set; }

        // Fiyat filtreleri
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        // Oda tipi filtresi
        public string? RoomType { get; set; } // "1+1", "2+1", "3+1", "4+1", "Dublex" vb.

        // Konum filtreleri
        public string? Street { get; set; }

        // Sıralama seçenekleri
        public string? SortBy { get; set; } // "price", "date", "title"
        public string? SortOrder { get; set; } = "asc"; // "asc", "desc"

        // Sayfalama
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}