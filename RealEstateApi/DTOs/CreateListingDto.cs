using System.ComponentModel.DataAnnotations;

namespace RealEstateApi.DTOs
{
    public class CreateListingDto
    {
        [Required(ErrorMessage = "Başlık gereklidir")]
        [MinLength(5, ErrorMessage = "Başlık en az 5 karakter olmalıdır")]
        [MaxLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Açıklama gereklidir")]
        [MinLength(10, ErrorMessage = "Açıklama en az 10 karakter olmalıdır")]
        [MaxLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şehir gereklidir")]
        [MinLength(2, ErrorMessage = "Şehir adı en az 2 karakter olmalıdır")]
        [MaxLength(50, ErrorMessage = "Şehir adı en fazla 50 karakter olabilir")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "İlçe gereklidir")]
        [MinLength(2, ErrorMessage = "İlçe adı en az 2 karakter olmalıdır")]
        [MaxLength(50, ErrorMessage = "İlçe adı en fazla 50 karakter olabilir")]
        public string District { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sokak bilgisi gereklidir")]
        [MinLength(2, ErrorMessage = "Sokak adı en az 2 karakter olmalıdır")]
        [MaxLength(100, ErrorMessage = "Sokak adı en fazla 100 karakter olabilir")]
        public string Street { get; set; } = string.Empty;

        [MaxLength(10, ErrorMessage = "Daire numarası en fazla 10 karakter olabilir")]
        public string? ApartmentNumber { get; set; }

        [Required(ErrorMessage = "Oda tipi gereklidir")]
        [RegularExpression(@"^(1\+1|2\+1|3\+1|4\+1|5\+1|6\+1|Dublex|Triplex|Villa|Stüdyo)$",
            ErrorMessage = "Geçerli oda tipleri: 1+1, 2+1, 3+1, 4+1, 5+1, 6+1, Dublex, Triplex, Villa, Stüdyo")]
        public string RoomType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Fiyat gereklidir")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır")]
        public decimal Price { get; set; }
    }
}