namespace RealEstateApi.Services
{
    using RealEstateApi.DTOs;
    using RealEstateApi.Models;
    using RealEstateApi.Persistence;
    using RealEstateApi.Helpers;
    using System.Text.RegularExpressions;

    public class AuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly JwtTokenHelper _jwt;

        public AuthService(ApplicationDbContext db, JwtTokenHelper jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        private bool ValidatePassword(string password, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            {
                errorMessage = "Password must be at least 8 characters long.";
                return false;
            }
            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                errorMessage = "Password must contain at least one uppercase letter.";
                return false;
            }
            if (!Regex.IsMatch(password, @"[a-z]"))
            {
                errorMessage = "Password must contain at least one lowercase letter.";
                return false;
            }
            if (!Regex.IsMatch(password, @"\d"))
            {
                errorMessage = "Password must contain at least one digit.";
                return false;
            }
            if (!Regex.IsMatch(password, @"[\W_]"))
            {
                errorMessage = "Password must contain at least one special character.";
                return false;
            }

            return true;
        }

        public List<User> GetAllUsers()
        {
            return _db.Users.ToList();
        }

        public AuthResult Register(RegisterDto dto)
        {
            if (_db.Users.Any(u => u.Email == dto.Email))
            {
                return new AuthResult(false, "Email is already in use.");
            }

            if (!ValidatePassword(dto.Password, out var passwordError))
            {
                return new AuthResult(false, passwordError);
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

        public AuthResult DeleteUserById(int id)
        {
            var user = _db.Users.Find(id);
            if (user == null)
            {
                return new AuthResult(false, "User not found.");
            }

            _db.Users.Remove(user);
            _db.SaveChanges();

            return new AuthResult(true, "User deleted.");
        }

        public AuthResult UpdateUser(int id, UpdateUserDto dto)
        {
            var user = _db.Users.Find(id);
            if (user == null)
            {
                return new AuthResult(false, "User not found.");
            }

            user.FullName = dto.FullName ?? user.FullName;
            user.Email = dto.Email ?? user.Email;

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                if (!ValidatePassword(dto.Password, out var passwordError))
                {
                    return new AuthResult(false, passwordError);
                }
                user.PasswordHash = PasswordHasher.Hash(dto.Password);
            }

            _db.Users.Update(user);
            _db.SaveChanges();

            return new AuthResult(true, "User updated.");
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
