using Entities.DTOs;
using Entities.Models;
using Repository.Interfaces;
using Services.Interfaces;
using Services.Security;
using System.Security.Cryptography;

namespace Services.Implementation
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _repo;
        private readonly JwtTokenGenerator _jwt;

        public AuthenticationService(IUserRepository repo, JwtTokenGenerator jwt)
        {
            _repo = repo;
            _jwt = jwt;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                if (await _repo.ExistsAsync(request.Email))
                    throw new Exception("User already exists.");

                var hash = HashPassword(request.Password);
                var user = new User { Email = request.Email, PasswordHash = hash };
                await _repo.CreateAsync(user);

                return _jwt.GenerateToken(user);
            }
            catch(Exception ex)
            {
                throw new Exception($"Registration failed: {ex.Message}");
            }   
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _repo.GetByEmailAsync(request.Email)
                ?? throw new Exception("Invalid credentials.");

                if (!VerifyPassword(request.Password, user.PasswordHash))
                    throw new Exception("Invalid credentials.");

                return _jwt.GenerateToken(user);
            }
            catch(Exception ex)
            {
                throw new Exception($"Login failed: {ex.Message}");
            }
        }

        private string HashPassword(string password)
        {
            try
            {
                using var deriveBytes = new Rfc2898DeriveBytes(password, 16, 10000, HashAlgorithmName.SHA256);
                byte[] salt = deriveBytes.Salt;
                byte[] hash = deriveBytes.GetBytes(32);

                return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
            }
            catch(Exception ex)
            {
                throw new Exception($"Password hashing failed: {ex.Message}");
            }
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            try
            {
                var parts = storedHash.Split(':');
                if (parts.Length != 2) return false;

                byte[] salt = Convert.FromBase64String(parts[0]);
                byte[] storedHashBytes = Convert.FromBase64String(parts[1]);

                using var deriveBytes = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
                byte[] computedHash = deriveBytes.GetBytes(32);

                return computedHash.SequenceEqual(storedHashBytes);
            }
            catch (Exception ex)
            {
                throw new Exception($"Password verification failed: {ex.Message}");
            }
        }
    }
}