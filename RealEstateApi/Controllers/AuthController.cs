namespace RealEstateApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using RealEstateApi.DTOs;
    using RealEstateApi.Services;

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Kullanıcı kaydı
        /// </summary>
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto dto)
        {
            try
            {
                _logger.LogInformation("Register attempt for email: {Email}", dto.Email);

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

                var result = _authService.Register(dto);

                if (!result.Success)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = result.Message
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = result.Message,
                    token = result.Token
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email: {Email}", dto.Email);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Sunucu hatası oluştu"
                });
            }
        }

        /// <summary>
        /// Kullanıcı girişi
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            try
            {
                _logger.LogInformation("Login attempt for email: {Email}", dto.Email);

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

                var result = _authService.Login(dto);

                if (!result.Success)
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = result.Message
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = result.Message,
                    token = result.Token
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", dto.Email);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Sunucu hatası oluştu"
                });
            }
        }

        /// <summary>
        /// API durumu kontrolü
        /// </summary>
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new
            {
                success = true,
                message = "API çalışıyor",
                timestamp = DateTime.UtcNow
            });
        }
    }
}