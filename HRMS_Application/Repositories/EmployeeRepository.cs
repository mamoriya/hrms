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

        public async Task CreateLeaveBalancesForEmployee( int employeeProfileId)
        {
            var leaveTypes = await _context.LeaveTypes.ToListAsync();

            var balances = leaveTypes.Select(leaveType => new LeaveBalance
            {
                EmployeeProfileId = employeeProfileId,
                LeaveTypeId = leaveType.Id,
                TotalAllotted = leaveType.AnnualQuota,
                Used = 0,
                Reserved = 0
            }).ToList();

            if (balances.Count > 0)
            {
                _context.LeaveBalances.AddRange(balances);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<bool> DepartmentExists(int departmentId)
        {
            return await _context.Departments
                .AnyAsync(d => d.Id == departmentId);
        }
        public async Task<User> CreateUser(User user)
        {
            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return user;
        }
        public async Task<EmployeeProfile> CreateEmployee(EmployeeProfile employee)
        {
            _context.EmployeeProfiles.Add(employee);

            await _context.SaveChangesAsync();

            return employee;
        }


        public async Task<EmployeeProfile> CreateEmployeeWithLeaveBalances( User user, EmployeeProfile employee)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Create User
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // 2. Create Employee Profile
                employee.UserId = user.Id;
                _context.EmployeeProfiles.Add(employee);
                await _context.SaveChangesAsync();

                // 3. Create Leave Balances for all Leave Types
                var leaveTypes = await _context.LeaveTypes.ToListAsync();

                var balances = leaveTypes.Select(leaveType =>
                    new LeaveBalance
                    {
                        EmployeeProfileId = employee.Id,
                        LeaveTypeId = leaveType.Id,
                        TotalAllotted = leaveType.AnnualQuota,
                        Used = 0,
                        Reserved = 0
                    }).ToList();

                if (balances.Count > 0)
                {
                    _context.LeaveBalances.AddRange(balances);
                    await _context.SaveChangesAsync();
                }

                // 4. Commit all changes together
                await transaction.CommitAsync();

                return employee;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
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
        public async Task<EmployeeProfile?> UpdateEmployee( int id, UpdateEmployeeRequest request)
        {
            var employee = await _context.EmployeeProfiles
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return null;
            }

            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.PhoneNumber = request.PhoneNumber;
            employee.Address = request.Address;
            employee.JobTitle = request.JobTitle;
            employee.DepartmentId = request.DepartmentId;
            employee.ManagerId = request.ManagerId;
            employee.JoiningDate = request.JoiningDate;

            await _context.SaveChangesAsync();

            return employee;
        }

        public async Task<EmployeeProfile?> GetEmployeeByUserId(int userId)
        {
            return await _context.EmployeeProfiles
                .FirstOrDefaultAsync(e => e.UserId == userId);
        }
        public async Task DeleteEmployee(EmployeeProfile employee)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == employee.UserId);

            _context.EmployeeProfiles.Remove(employee);

            if (user != null)
            {
                _context.Users.Remove(user);
            }

        }
        public async Task<bool> HasDirectReports(int employeeId)
        {
            return await _context.EmployeeProfiles
                .AnyAsync(e => e.ManagerId == employeeId);
        }
        public async Task<List<EmployeeProfile>> GetDirectReports(int managerId)
        {
            return await _context.EmployeeProfiles
                    .Include(e => e.User)
                    .Include(e => e.Department)
                    .Where(e => e.ManagerId == managerId)
                    .ToListAsync();
        }
        public async Task ReassignDirectReports( int oldManagerId, int newManagerId)
        {
            var employees = await _context.EmployeeProfiles
                .Where(e => e.ManagerId == oldManagerId)
                .ToListAsync();

            foreach (var employee in employees)
            {
                employee.ManagerId = newManagerId;
            }

           
        }

        public async Task DeleteEmployeeDocuments(int employeeId)
        {
            var documents = await _context.EmployeeDocuments
                .Where(d => d.EmployeeProfileId == employeeId)
                .ToListAsync();

            _context.EmployeeDocuments.RemoveRange(documents);

           
        }

        public async Task DeleteLeaveBalances(int employeeId)
        {
            var balances = await _context.LeaveBalances
                .Where(b => b.EmployeeProfileId == employeeId)
                .ToListAsync();

            _context.LeaveBalances.RemoveRange(balances);
        }

        public async Task DeleteLeaveRequests(int employeeId)
        {
            var requests = await _context.LeaveRequests
                .Where(r => r.EmployeeProfileId == employeeId)
                .ToListAsync();

            _context.LeaveRequests.RemoveRange(requests);

            
        }

        public async Task<EmployeeProfile?> GetManagerById(int managerId)
        {
            return await _context.EmployeeProfiles
                .Include(e => e.User)
                .FirstOrDefaultAsync(e =>
                    e.Id == managerId &&
                    e.User.Role == "Manager");
        }
        public async Task<bool> HasDocuments(int employeeId)
        {
            return await _context.EmployeeDocuments
                .AnyAsync(d => d.EmployeeProfileId == employeeId);
        }

        public async Task ClearManagerFromDirectReports(int managerId)
        {
            var employees = await _context.EmployeeProfiles
                .Where(e => e.ManagerId == managerId)
                .ToListAsync();

            foreach (var employee in employees)
            {
                employee.ManagerId = null;
            }
        }

        public async Task DeleteEmployeeWithRelatedData(int employeeId, int? newManagerId = null)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                if (newManagerId.HasValue)
                {
                    await ReassignDirectReports(
                        employeeId,
                        newManagerId.Value
                    );

                    await _context.SaveChangesAsync();
                }
                else
                {
                    await ClearManagerFromDirectReports(employeeId);

                    await _context.SaveChangesAsync();
                }

                // 2. Related records delete karo
                await DeleteEmployeeDocuments(employeeId);
                await DeleteLeaveBalances(employeeId);
                await DeleteLeaveRequests(employeeId);

                // 3. Employee + User delete karo
                var employee = await GetEmployeeById(employeeId);

                if (employee != null)
                {
                    await DeleteEmployee(employee);
                }

                // 4. Remaining changes save karo
                await _context.SaveChangesAsync();

                // 5. Everything successful
                await transaction.CommitAsync();
            }
            catch
            {
                // Koi bhi error aaye to complete transaction rollback
                await transaction.RollbackAsync();

                throw;
            }
        }
    }
}
