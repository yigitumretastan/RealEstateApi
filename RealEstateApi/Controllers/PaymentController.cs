using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RealEstateApi.DTOs;
using RealEstateApi.Models;
using RealEstateApi.Services;
using System.Linq;
using System.Security.Claims;

namespace RealEstateApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(PaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Ödeme işlemi gerçekleştirir ve veritabanına kaydeder.
        /// </summary>
        [Authorize]
        [HttpPost("process")]
        public IActionResult ProcessPayment([FromBody] PaymentDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new
                    {
                        success = false,
                        message = "Geçersiz veriler",
                        errors = errors
                    });
                }

                var userIdFromToken = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                if (userIdFromToken != dto.UserId)
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Kullanıcı yetkisi yok."
                    });
                }

                // Sadece kredi kartı ödeme yöntemi kabul edilecek
                if (dto.PaymentMethod != "CreditCard")
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Sadece kredi kartı ile ödeme yapılabilir."
                    });
                }

                if (string.IsNullOrEmpty(dto.CardNumber) ||
                    string.IsNullOrEmpty(dto.CardHolderName) ||
                    string.IsNullOrEmpty(dto.ExpiryDate) ||
                    string.IsNullOrEmpty(dto.CVV))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Kredi kartı ödemeleri için tüm kart bilgileri gereklidir."
                    });
                }

                var payment = _paymentService.ProcessPayment(dto.ListingId, dto);

                if (payment == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "İlan bulunamadı, ödeme yapılamıyor."
                    });
                }

                _paymentService.SavePayment(payment);

                var safePaymentData = new
                {
                    id = payment.Id,
                    transactionId = payment.TransactionId,
                    amount = payment.Amount,
                    paymentMethod = payment.PaymentMethod,
                    maskedCardNumber = payment.MaskedCardNumber,
                    cardHolderName = payment.CardHolderName,
                    isSuccessful = payment.IsSuccessful,
                    failureReason = payment.FailureReason,
                    createdAt = payment.CreatedAt
                };

                if (!payment.IsSuccessful)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Ödeme başarısız.",
                        data = safePaymentData
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Ödeme başarıyla gerçekleştirildi",
                    data = safePaymentData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ödeme sırasında bir hata oluştu.");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Sunucu hatası meydana geldi"
                });
            }
        }

        /// <summary>
        /// Kullanıcının ödeme geçmişini getirir
        /// </summary>
        [Authorize]
        [HttpGet("history")]
        public IActionResult GetPaymentHistory()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var payments = _paymentService.GetUserPayments(userId);

                var safePayments = payments.Select(p => new
                {
                    id = p.Id,
                    transactionId = p.TransactionId,
                    amount = p.Amount,
                    paymentMethod = p.PaymentMethod,
                    maskedCardNumber = p.MaskedCardNumber,
                    cardHolderName = p.CardHolderName,
                    isSuccessful = p.IsSuccessful,
                    failureReason = p.FailureReason,
                    createdAt = p.CreatedAt,
                    description = p.Description
                });

                return Ok(new
                {
                    success = true,
                    data = safePayments
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ödeme geçmişi getirilirken hata oluştu.");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Sunucu hatası meydana geldi"
                });
            }
        }

        /// <summary>
        /// Belirli bir ödemenin detaylarını getirir
        /// </summary>
        [Authorize]
        [HttpGet("{paymentId}")]
        public IActionResult GetPaymentDetails(int paymentId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var payment = _paymentService.GetPaymentById(paymentId, userId);

                if (payment == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Ödeme bulunamadı"
                    });
                }

                var safePaymentData = new
                {
                    id = payment.Id,
                    transactionId = payment.TransactionId,
                    amount = payment.Amount,
                    paymentMethod = payment.PaymentMethod,
                    maskedCardNumber = payment.MaskedCardNumber,
                    cardHolderName = payment.CardHolderName,
                    billingAddress = payment.BillingAddress,
                    billingCity = payment.BillingCity,
                    postalCode = payment.PostalCode,
                    isSuccessful = payment.IsSuccessful,
                    failureReason = payment.FailureReason,
                    createdAt = payment.CreatedAt,
                    description = payment.Description
                };

                return Ok(new
                {
                    success = true,
                    data = safePaymentData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ödeme detayları getirilirken hata oluştu. PaymentId: {PaymentId}", paymentId);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Sunucu hatası meydana geldi"
                });
            }
        }

        /// <summary>
        /// Kart numarası doğrulama (frontend için yardımcı endpoint)
        /// </summary>
        [HttpPost("validate-card")]
        public IActionResult ValidateCard([FromBody] CardValidationDto dto)
        {
            try
            {
                var isValid = !string.IsNullOrEmpty(dto.CardNumber) &&
                             dto.CardNumber.Replace(" ", "").Replace("-", "").Length >= 13;

                return Ok(new
                {
                    success = true,
                    isValid = isValid,
                    cardType = GetCardType(dto.CardNumber)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kart doğrulama sırasında hata oluştu.");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Sunucu hatası meydana geldi"
                });
            }
        }

        private string GetCardType(string? cardNumber)
        {
            if (string.IsNullOrEmpty(cardNumber))
                return "Unknown";

            var cleanNumber = cardNumber.Replace(" ", "").Replace("-", "");

            if (cleanNumber.StartsWith("4"))
                return "Visa";
            else if (cleanNumber.StartsWith("5") || cleanNumber.StartsWith("2"))
                return "MasterCard";
            else if (cleanNumber.StartsWith("3"))
                return "American Express";
            else
                return "Unknown";
        }
    }

    public class CardValidationDto
    {
        public string? CardNumber { get; set; }
    }
}
