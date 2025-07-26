using System.ComponentModel.DataAnnotations;

namespace Real_Estate_Api.DTOs
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

        [Required(ErrorMessage = "Oda sayısı gereklidir")]
        [Range(1, 20, ErrorMessage = "Oda sayısı 1 ile 20 arasında olmalıdır")]
        public int RoomCount { get; set; }

        [Required(ErrorMessage = "Fiyat gereklidir")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır")]
        public decimal Price { get; set; }
    }
}