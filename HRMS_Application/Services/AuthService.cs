using HRMS_Application.DTOs;
using HRMS_Application.Models;
using HRMS_Application.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace HRMS_Application.Services
{
    public class AuthService
    {
        private readonly AuthRepository _repository;
        private readonly IConfiguration _configuration;

        public AuthService(AuthRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;

        }
        public async Task<LoginResponse?> Login(LoginRequest request)
        {
            if (request == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return null;
            }

            var user = await _repository.GetUserByEmail(request.Email);

            if (user == null)
            {
                return null;
            }

            if (user.Password != request.Password)
            {
                return null;
            }

            var token = GenerateJwtToken(user);

            return new LoginResponse
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role,
                Token = token
            };
        }

        public async Task<User?> SetupPassword(SetupPasswordRequest request)
        {
            if (request == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return null;
            }

            var user = await _repository.UpdatePassword(
                request.Email,
                request.Password);

            return user;
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role)
        };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!
                )
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
