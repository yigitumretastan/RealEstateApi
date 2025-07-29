using System.ComponentModel.DataAnnotations;

namespace RealEstateApi.DTOs
{
    public class PaymentDto
    {
        [Required(ErrorMessage = "Kullanıcı ID zorunludur.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "İlan ID zorunludur.")]
        public int ListingId { get; set; }  // Eklendi

        [Required(ErrorMessage = "Ödeme yöntemi belirtilmelidir.")]
        public string PaymentMethod { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
