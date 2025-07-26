namespace Real_Estate_Api.Services
{
    using Real_Estate_Api.DTOs;
    using Real_Estate_Api.Models;
    using Real_Estate_Api.Persistence;
    using Real_Estate_Api.Helpers;

    public class AuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly JwtTokenHelper _jwt;

        public AuthService(ApplicationDbContext db, JwtTokenHelper jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        public AuthResult Register(RegisterDto dto)
        {
            if (_db.Users.Any(u => u.Email == dto.Email))
            {
                return new AuthResult(false, "Email is already in use.");
            }

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = PasswordHasher.Hash(dto.Password)
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            var token = _jwt.GenerateToken(user);
            return new AuthResult(true, "User registered.", token);
        }

        public AuthResult Login(LoginDto dto)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == dto.Email);
            if (user == null || !PasswordHasher.Verify(dto.Password, user.PasswordHash))
            {
                return new AuthResult(false, "Invalid credentials.");
            }

            var token = _jwt.GenerateToken(user);
            return new AuthResult(true, "Login successful.", token);
        }
    }

    public class AuthResult
    {
        public bool Success { get; }
        public string Message { get; }
        public string? Token { get; }

        public AuthResult(bool success, string message, string? token = null)
        {
            Success = success;
            Message = message;
            Token = token;
        }
    }

}
