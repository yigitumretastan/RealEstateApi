using System.ComponentModel.DataAnnotations;

namespace Real_Estate_Api.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Ad Soyad gereklidir")]
        [MinLength(2, ErrorMessage = "Ad Soyad en az 2 karakter olmalıdır")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email gereklidir")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre gereklidir")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
        public string Password { get; set; } = string.Empty;
    }
}   