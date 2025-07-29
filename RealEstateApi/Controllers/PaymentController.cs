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
        [HttpPost("Payment")]
        public IActionResult Payment([FromBody] PaymentDto dto)
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

                // Kullanıcı id'yi token'dan al (güvenlik için)
                var userIdFromToken = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                if (userIdFromToken != dto.UserId)
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Kullanıcı yetkisi yok."
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

                if (!payment.IsSuccessful)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Ödeme başarısız. Tutar geçersiz olabilir."
                    });
                }

                // Ödemeyi veritabanına kaydet
                _paymentService.SavePayment(payment);

                return Ok(new
                {
                    success = true,
                    message = "Ödeme başarıyla gerçekleştirildi",
                    data = payment
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
    }
}
