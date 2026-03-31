using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BusinessLayer.Interfaces;
using DataAccessLayer.Data;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ModelLayer.DTOs;
using ModelLayer.Models;

namespace BusinessLayer.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public UserService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // =====================
        //    REGISTER (local)
        // =====================
        public AuthResponseDto Register(RegisterDto dto)
        {
            var existingUser = _context.Users
                .FirstOrDefault(u => u.Email == dto.Email);

            if (existingUser != null)
                throw new Exception("Email already registered.");

            string salt = GenerateSalt();
            string hashedPassword = HashPassword(dto.Password, salt);

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Salt = salt,
                HashedPassword = hashedPassword,
                AuthProvider = "local"
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                ProfilePicture = user.ProfilePicture,
                Message = "Registration successful!"
            };
        }

        // =====================
        //    LOGIN (local)
        // =====================
        public AuthResponseDto Login(LoginDto dto)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.Email == dto.Email && u.AuthProvider == "local");

            if (user == null)
                throw new Exception("Invalid email or password.");

            string hashedAttempt = HashPassword(dto.Password, user.Salt!);

            if (hashedAttempt != user.HashedPassword)
                throw new Exception("Invalid email or password.");

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                ProfilePicture = user.ProfilePicture,
                Message = "Login successful!"
            };
        }

        // ========================
        //    GOOGLE LOGIN / SIGNUP
        // ========================
        public async Task<AuthResponseDto> GoogleLoginAsync(GoogleAuthDto dto)
        {
            // 1. Google ke ID token ko verify karo
            var payload = await VerifyGoogleTokenAsync(dto.IdToken);

            // 2. Agar already exist karta hai Google user toh login karo
            var user = _context.Users
                .FirstOrDefault(u => u.GoogleId == payload.Subject);

            if (user == null)
            {
                // 3. Email se check karo — same email se local account toh nahi hai?
                var emailUser = _context.Users
                    .FirstOrDefault(u => u.Email == payload.Email);

                if (emailUser != null && emailUser.AuthProvider == "local")
                    throw new Exception("This email is already registered with a password. Please login with email & password.");

                // 4. Naya Google user create karo
                user = new User
                {
                    FirstName = payload.GivenName ?? payload.Name ?? "User",
                    LastName = payload.FamilyName ?? "",
                    Email = payload.Email,
                    GoogleId = payload.Subject,
                    ProfilePicture = payload.Picture,
                    AuthProvider = "google"
                };

                _context.Users.Add(user);
                _context.SaveChanges();
            }
            else
            {
                // 5. Profile picture update karo agar change hua ho
                user.ProfilePicture = payload.Picture;
                _context.SaveChanges();
            }

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                UserId = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                ProfilePicture = user.ProfilePicture,
                Message = "Google login successful!"
            };
        }

        // ==============================
        //    HELPER: Google Token Verify
        // ==============================
        private async Task<GoogleJsonWebSignature.Payload> VerifyGoogleTokenAsync(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _config["Google:ClientId"] }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
                return payload;
            }
            catch (InvalidJwtException)
            {
                throw new Exception("Invalid Google token.");
            }
        }

        // =====================
        //    HELPER: Salt banao
        // =====================
        private string GenerateSalt()
        {
            byte[] saltBytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(saltBytes);
        }

        // ==========================
        //    HELPER: Password hash
        // ==========================
        private string HashPassword(string password, string salt)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] combined = passwordBytes.Concat(saltBytes).ToArray();
            byte[] hashBytes = SHA512.HashData(combined);
            return Convert.ToBase64String(hashBytes);
        }

        // ==========================
        //    HELPER: JWT Token banao
        // ==========================
        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("FirstName", user.FirstName),
                new Claim("AuthProvider", user.AuthProvider)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_config["Jwt:ExpiryMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
