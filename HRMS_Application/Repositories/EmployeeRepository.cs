using HRMS_Application.Data;
using HRMS_Application.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS_Application.Repositories
{
    public class EmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> EmailExists(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email);
        }
        public async Task<bool> DepartmentExists(int departmentId)
        {
            return await _context.Departments
                .AnyAsync(d => d.Id == departmentId);
        }
        public async Task<EmployeeProfile> CreateEmployee(EmployeeProfile employee)
        {
            _context.EmployeeProfiles.Add(employee);

            await _context.SaveChangesAsync();

            return employee;
        }
        public async Task<List<EmployeeProfile>> GetAllEmployees()
        {
            return await _context.EmployeeProfiles
                .Include(e => e.User)
                .ToListAsync();
        }
        public async Task<EmployeeProfile?> GetEmployeeById(int id)
        {
            return await _context.EmployeeProfiles
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<EmployeeProfile> UpdateEmployee(EmployeeProfile employee)
        {
            _context.EmployeeProfiles.Update(employee);

            await _context.SaveChangesAsync();

            return employee;
        }
    }
}
