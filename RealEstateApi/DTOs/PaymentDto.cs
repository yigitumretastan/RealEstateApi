using System.ComponentModel.DataAnnotations;

namespace RealEstateApi.DTOs
{
    public class PaymentDto
    {
        [Required(ErrorMessage = "Kullanıcı ID zorunludur.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "İlan ID zorunludur.")]
        public int ListingId { get; set; }

        [Required(ErrorMessage = "Ödeme yöntemi belirtilmelidir.")]
        [RegularExpression(@"^CreditCard$", 
            ErrorMessage = "Sadece 'CreditCard' ödeme yöntemi geçerlidir.")]
        public string PaymentMethod { get; set; } = "CreditCard";

        [Required(ErrorMessage = "Kart numarası zorunludur.")]
        [CreditCard(ErrorMessage = "Geçerli bir kart numarası giriniz.")]
        [StringLength(19, MinimumLength = 13, ErrorMessage = "Kart numarası 13-19 karakter arasında olmalıdır.")]
        public string? CardNumber { get; set; }

        [Required(ErrorMessage = "Kart sahibi adı zorunludur.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Kart sahibi adı 2-50 karakter arasında olmalıdır.")]
        public string? CardHolderName { get; set; }

        [Required(ErrorMessage = "Son kullanma tarihi zorunludur.")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/([0-9]{2})$", 
            ErrorMessage = "Son kullanma tarihi MM/YY formatında olmalıdır.")]
        public string? ExpiryDate { get; set; }

        [Required(ErrorMessage = "CVV kodu zorunludur.")]
        [RegularExpression(@"^[0-9]{3,4}$", ErrorMessage = "CVV 3-4 haneli sayı olmalıdır.")]
        public string? CVV { get; set; }

        [Required(ErrorMessage = "Fatura adresi zorunludur.")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "Fatura adresi 10-200 karakter arasında olmalıdır.")]
        public string? BillingAddress { get; set; }

        [Required(ErrorMessage = "Şehir bilgisi zorunludur.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Şehir adı 2-50 karakter arasında olmalıdır.")]
        public string? BillingCity { get; set; }

        [Required(ErrorMessage = "Posta kodu zorunludur.")]
        [RegularExpression(@"^[0-9]{5}$", ErrorMessage = "Posta kodu 5 haneli olmalıdır.")]
        public string? PostalCode { get; set; }

        public string? Description { get; set; }

        public void ClearSensitiveData()
        {
            if (!string.IsNullOrEmpty(CardNumber))
            {
                CardNumber = "**** **** **** " + CardNumber.Substring(Math.Max(0, CardNumber.Length - 4));
            }
            CVV = "***";
        }
    }
}
