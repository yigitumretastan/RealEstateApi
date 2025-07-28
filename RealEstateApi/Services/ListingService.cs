namespace RealEstateApi.Services
{
    using RealEstateApi.DTOs;
    using RealEstateApi.Models;
    using RealEstateApi.Persistence;
    using System.Security.Claims;
    using Microsoft.EntityFrameworkCore;

    public class ListingService
    {
        private readonly ApplicationDbContext _db;

        public ListingService(ApplicationDbContext db)
        {
            _db = db;
        }

        public ListingResult GetFilteredListings(ListingFilterDto filter)
        {
            var query = _db.Listings.Include(l => l.User).AsQueryable();

            // Metin tabanlı arama (başlık ve açıklamada)
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.ToLower();
                query = query.Where(x =>
                    x.Title.ToLower().Contains(searchTerm) ||
                    x.Description.ToLower().Contains(searchTerm));
            }

            // Şehir filtresi
            if (!string.IsNullOrWhiteSpace(filter.City))
                query = query.Where(x => x.City.ToLower() == filter.City.ToLower());

            // İlçe filtresi
            if (!string.IsNullOrWhiteSpace(filter.District))
                query = query.Where(x => x.District.ToLower() == filter.District.ToLower());

            // Sokak filtresi
            if (!string.IsNullOrWhiteSpace(filter.Street))
                query = query.Where(x => x.Street.ToLower().Contains(filter.Street.ToLower()));

            // Fiyat filtreleri
            if (filter.MinPrice.HasValue)
                query = query.Where(x => x.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(x => x.Price <= filter.MaxPrice.Value);

            // Oda tipi filtresi
            if (!string.IsNullOrWhiteSpace(filter.RoomType))
                query = query.Where(x => x.RoomType == filter.RoomType);

            // Toplam kayıt sayısı
            var totalCount = query.Count();

            // Sıralama
            query = filter.SortBy?.ToLower() switch
            {
                "price" => filter.SortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(x => x.Price)
                    : query.OrderBy(x => x.Price),
                "date" => filter.SortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(x => x.CreatedAt)
                    : query.OrderBy(x => x.CreatedAt),
                "title" => filter.SortOrder?.ToLower() == "desc"
                    ? query.OrderByDescending(x => x.Title)
                    : query.OrderBy(x => x.Title),
                _ => query.OrderByDescending(x => x.CreatedAt) // Varsayılan: yeni ilanlar önce
            };

            // Sayfalama
            var listings = query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            return new ListingResult
            {
                Listings = listings,
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize)
            };
        }

        public Listing? GetListingById(int id)
        {
            return _db.Listings
                .Include(l => l.User)
                .FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Listing> GetUserListings(int userId)
        {
            return _db.Listings
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
        }

        public Listing CreateListing(CreateListingDto dto, ClaimsPrincipal userClaims)
        {
            var userId = int.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var listing = new Listing
            {
                Title = dto.Title,
                Description = dto.Description,
                City = dto.City,
                District = dto.District,
                Street = dto.Street,
                ApartmentNumber = dto.ApartmentNumber,
                RoomType = dto.RoomType,
                Price = dto.Price,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Listings.Add(listing);
            _db.SaveChanges();

            return listing;
        }

        public ServiceResult UpdateListing(int id, UpdateListingDto dto, ClaimsPrincipal userClaims)
        {
            var listing = _db.Listings.FirstOrDefault(x => x.Id == id);
            if (listing == null)
                return new ServiceResult(false, "İlan bulunamadı");

            var userId = int.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (listing.UserId != userId)
                return new ServiceResult(false, "Bu ilanı düzenleme yetkiniz yok");

            // Güncelleme işlemleri
            if (!string.IsNullOrWhiteSpace(dto.Title))
                listing.Title = dto.Title;

            if (!string.IsNullOrWhiteSpace(dto.Description))
                listing.Description = dto.Description;

            if (!string.IsNullOrWhiteSpace(dto.City))
                listing.City = dto.City;

            if (!string.IsNullOrWhiteSpace(dto.District))
                listing.District = dto.District;

            if (!string.IsNullOrWhiteSpace(dto.Street))
                listing.Street = dto.Street;

            if (dto.ApartmentNumber != null)
                listing.ApartmentNumber = dto.ApartmentNumber;

            if (!string.IsNullOrWhiteSpace(dto.RoomType))
                listing.RoomType = dto.RoomType;

            if (dto.Price.HasValue)
                listing.Price = dto.Price.Value;

            listing.UpdatedAt = DateTime.UtcNow;

            _db.SaveChanges();

            return new ServiceResult(true, "İlan başarıyla güncellendi");
        }

        public ServiceResult DeleteListing(int id, ClaimsPrincipal userClaims)
        {
            var listing = _db.Listings.FirstOrDefault(x => x.Id == id);
            if (listing == null)
                return new ServiceResult(false, "İlan bulunamadı");

            var userId = int.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (listing.UserId != userId)
                return new ServiceResult(false, "Bu ilanı silme yetkiniz yok");

            _db.Listings.Remove(listing);
            _db.SaveChanges();

            return new ServiceResult(true, "İlan başarıyla silindi");
        }

        // İstatistikler için yardımcı method
        public ListingStats GetListingStats()
        {
            var totalListings = _db.Listings.Count();
            var avgPrice = _db.Listings.Any() ? _db.Listings.Average(x => (double)x.Price) : 0;

            var cityStats = _db.Listings
                .GroupBy(x => x.City)
                .Select(g => new CityStats
                {
                    City = g.Key,
                    Count = g.Count(),
                    AvgPrice = g.Average(x => (double)x.Price)
                })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToList();

            var roomTypeStats = _db.Listings
                .GroupBy(x => x.RoomType)
                .Select(g => new RoomTypeStats
                {
                    RoomType = g.Key,
                    Count = g.Count(),
                    AvgPrice = g.Average(x => (double)x.Price)
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            return new ListingStats
            {
                TotalListings = totalListings,
                AveragePrice = avgPrice,
                TopCities = cityStats,
                RoomTypeStats = roomTypeStats
            };
        }
    }

    public class ServiceResult
    {
        public bool Success { get; }
        public string Message { get; }

        public ServiceResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }

    public class ListingResult
    {
        public List<Listing> Listings { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class ListingStats
    {
        public int TotalListings { get; set; }
        public double AveragePrice { get; set; }
        public List<CityStats> TopCities { get; set; } = new();
        public List<RoomTypeStats> RoomTypeStats { get; set; } = new();
    }

    public class CityStats
    {
        public string City { get; set; } = string.Empty;
        public int Count { get; set; }
        public double AvgPrice { get; set; }
    }

    public class RoomTypeStats
    {
        public string RoomType { get; set; } = string.Empty;
        public int Count { get; set; }
        public double AvgPrice { get; set; }
    }
}