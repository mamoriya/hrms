using HRMS_Application.Data;
using HRMS_Application.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS_Application.Repositories
{
    public class AuthRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> UpdatePassword(string email, string password)
        {
            var user = await GetUserByEmail(email);

            if (user == null)
            {
                return null;
            }

            user.Password = password;

            await _context.SaveChangesAsync();

            return user;
        }
    }
}
