using System.ComponentModel.DataAnnotations;

namespace RealEstateApi.DTOs
{
    public class CreateListingDto
    {
        [Required(ErrorMessage = "Title is required")]
        [MinLength(5, ErrorMessage = "Title must be at least 5 characters long")]
        [MaxLength(200, ErrorMessage = "Title can be at most 200 characters long")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [MinLength(10, ErrorMessage = "Description must be at least 10 characters long")]
        [MaxLength(1000, ErrorMessage = "Description can be at most 1000 characters long")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        [MinLength(2, ErrorMessage = "City name must be at least 2 characters long")]
        [MaxLength(50, ErrorMessage = "City name can be at most 50 characters long")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "District is required")]
        [MinLength(2, ErrorMessage = "District name must be at least 2 characters long")]
        [MaxLength(50, ErrorMessage = "District name can be at most 50 characters long")]
        public string District { get; set; } = string.Empty;

        [Required(ErrorMessage = "Street is required")]
        [MinLength(2, ErrorMessage = "Street name must be at least 2 characters long")]
        [MaxLength(100, ErrorMessage = "Street name can be at most 100 characters long")]
        public string Street { get; set; } = string.Empty;

        [MaxLength(10, ErrorMessage = "Apartment number can be at most 10 characters long")]
        public string? ApartmentNumber { get; set; }

        [Required(ErrorMessage = "Room type is required")]
        [RegularExpression(@"^(1\+1|2\+1|3\+1|4\+1|5\+1|6\+1|Dublex|Triplex|Villa|Stüdyo)$",
            ErrorMessage = "Valid room types: 1+1, 2+1, 3+1, 4+1, 5+1, 6+1, Dublex, Triplex, Villa, Stüdyo")]
        public string RoomType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
    }
}
