namespace Real_Estate_Api.Services
{
    using Real_Estate_Api.DTOs;
    using Real_Estate_Api.Models;
    using Real_Estate_Api.Persistence;
    using System.Security.Claims;

    public class ListingService
    {
        private readonly ApplicationDbContext _db;

        public ListingService(ApplicationDbContext db)
        {
            _db = db;
        }

        public IEnumerable<Listing> GetFilteredListings(ListingFilterDto filter)
        {
            var query = _db.Listings.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.City))
                query = query.Where(x => x.City == filter.City);

            if (filter.MinPrice.HasValue)
                query = query.Where(x => x.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(x => x.Price <= filter.MaxPrice.Value);

            if (filter.RoomCount.HasValue)
                query = query.Where(x => x.RoomCount == filter.RoomCount.Value);

            return query.ToList();
        }

        public Listing CreateListing(CreateListingDto dto, ClaimsPrincipal userClaims)
        {
            var userId = int.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var listing = new Listing
            {
                Title = dto.Title,
                Description = dto.Description,
                City = dto.City,
                Price = dto.Price,
                RoomCount = dto.RoomCount,
                UserId = userId
            };

            _db.Listings.Add(listing);
            _db.SaveChanges();

            return listing;
        }

        public ServiceResult UpdateListing(int id, UpdateListingDto dto, ClaimsPrincipal userClaims)
        {
            var listing = _db.Listings.FirstOrDefault(x => x.Id == id);
            if (listing == null) return new ServiceResult(false, "Listing not found");

            var userId = int.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (listing.UserId != userId) return new ServiceResult(false, "Unauthorized");

            listing.Title = dto.Title ?? listing.Title;
            listing.Description = dto.Description ?? listing.Description;
            listing.City = dto.City ?? listing.City;
            listing.Price = dto.Price ?? listing.Price;
            listing.RoomCount = dto.RoomCount ?? listing.RoomCount;

            _db.SaveChanges();

            return new ServiceResult(true, "Listing updated");
        }

        public ServiceResult DeleteListing(int id, ClaimsPrincipal userClaims)
        {
            var listing = _db.Listings.FirstOrDefault(x => x.Id == id);
            if (listing == null) return new ServiceResult(false, "Listing not found");

            var userId = int.Parse(userClaims.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (listing.UserId != userId) return new ServiceResult(false, "Unauthorized");

            _db.Listings.Remove(listing);
            _db.SaveChanges();

            return new ServiceResult(true, "Listing deleted");
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

}
