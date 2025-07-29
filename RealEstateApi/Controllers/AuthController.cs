namespace RealEstateApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using RealEstateApi.DTOs;
    using RealEstateApi.Services;
    using System.Security.Claims;

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
        /// User registration
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
        /// User login
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
        /// Delete user by ID
        /// </summary>
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                if (userId != id)
                {
                    return Forbid();
                }

                var result = _authService.DeleteUserById(id);

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
                    message = result.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user. ID: {Id}", id);
                return StatusCode(500, new
                {
                    success = false,
                    message = "A server error occurred"
                });
            }
        }
        /// <summary>
        /// Update user by ID
        /// </summary>
        [HttpPut("update/{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                if (userId != id)
                {
                    return Forbid();
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid input data",
                        errors = errors
                    });
                }

                var result = _authService.UpdateUser(id, dto);

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
                    message = result.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user. ID: {Id}", id);
                return StatusCode(500, new
                {
                    success = false,
                    message = "An unexpected server error occurred"
                });
            }

        }
    }
}