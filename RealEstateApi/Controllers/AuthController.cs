namespace RealEstateApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Real_Estate_Api.DTOs;
    using Real_Estate_Api.Services;

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(AuthService authService) : ControllerBase
    {
        private readonly AuthService _authService = authService;

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto dto)
        {
            var result = _authService.Register(dto);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            var result = _authService.Login(dto);
            if (!result.Success)
                return Unauthorized(result.Message);

            return Ok(result);
        }
    }
}
