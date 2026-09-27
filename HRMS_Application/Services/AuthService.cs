using HRMS_Application.DTOs;
using HRMS_Application.Models;
using HRMS_Application.Repositories;

namespace HRMS_Application.Services
{
    public class AuthService
    {
        private readonly AuthRepository _repository;

        public AuthService(AuthRepository repository)
        {
            _repository = repository;
        }
        public async Task<User?> Login(LoginRequest request)
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

            return user;
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
    }
}
