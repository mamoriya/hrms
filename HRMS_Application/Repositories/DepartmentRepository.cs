using HRMS_Application.Data;
using HRMS_Application.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS_Application.Repositories
{
    public class DepartmentRepository
    {
        private readonly ApplicationDbContext _context;

        public DepartmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Department> CreateDepartment(Department department)
        {
            _context.Departments.Add(department);

            await _context.SaveChangesAsync();

            return department;
        }

        public async Task<List<Department>> GetAllDepartments()
        {
            return await _context.Departments
                .ToListAsync();
        }

        public async Task<Department?> GetDepartmentById(int id)
        {
            return await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Department> UpdateDepartment(Department department)
        {
            _context.Departments.Update(department);

            await _context.SaveChangesAsync();

            return department;
        }

        public async Task DeleteDepartment(Department department)
        {
            _context.Departments.Remove(department);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> NameExists(string name)
        {
            return await _context.Departments
                .AnyAsync(d => d.Name == name);
        }
    }
}
