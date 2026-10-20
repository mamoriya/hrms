using HRMS_Application.Data;
using HRMS_Application.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS_Application.Repositories
{
    public class LeaveRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public LeaveRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<int?> GetEmployeeProfileIdByUserId(int userId)
        {
            var employee = await _context.EmployeeProfiles
                .FirstOrDefaultAsync(e => e.UserId == userId);

            return employee?.Id;
        }
        public async Task<bool> EmployeeExists(int employeeId)
        {
            return await _context.EmployeeProfiles
                .AnyAsync(e => e.Id == employeeId);
        }

        public async Task<bool> LeaveTypeExists(int leaveTypeId)
        {
            return await _context.LeaveTypes
                .AnyAsync(l => l.Id == leaveTypeId);
        }

        public async Task<LeaveBalance?> GetLeaveBalance(
            int employeeId,
            int leaveTypeId)
        {
            return await _context.LeaveBalances
                .FirstOrDefaultAsync(b =>
                    b.EmployeeProfileId == employeeId &&
                    b.LeaveTypeId == leaveTypeId);
        }

        public async Task<List<LeaveRequest>> GetOverlappingRequests(
            int employeeId,
            DateTime startDate,
            DateTime endDate)
        {
            return await _context.LeaveRequests
                .Where(r =>
                    r.EmployeeProfileId == employeeId &&
                    (r.Status == "PENDING" ||
                     r.Status == "APPROVED") &&
                    r.StartDate <= endDate &&
                    r.EndDate >= startDate)
                .ToListAsync();
        }

        public async Task<LeaveRequest> CreateLeaveRequest(
            LeaveRequest leaveRequest)
        {
            _context.LeaveRequests.Add(leaveRequest);

            await _context.SaveChangesAsync();

            return leaveRequest;
        }

        public async Task<LeaveRequest> CreateLeaveRequestWithBalance( LeaveRequest leaveRequest,  LeaveBalance balance)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                _context.LeaveBalances.Update(balance);
                _context.LeaveRequests.Add(leaveRequest);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return leaveRequest;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task<LeaveRequest?> GetLeaveRequestById(int id)
        {
            return await _context.LeaveRequests
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task UpdateLeaveRequest(
            LeaveRequest leaveRequest)
        {
            _context.LeaveRequests.Update(leaveRequest);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateLeaveBalance(
            LeaveBalance balance)
        {
            _context.LeaveBalances.Update(balance);

            await _context.SaveChangesAsync();
        }


        public async Task<LeaveRequest> UpdateLeaveRequestWithBalance(
            LeaveRequest leaveRequest,
            LeaveBalance balance)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                _context.LeaveBalances.Update(balance);
                _context.LeaveRequests.Update(leaveRequest);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return leaveRequest;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }



        public async Task<List<LeaveRequest>> GetAllLeaveRequests()
        {
            return await _context.LeaveRequests
                .AsNoTracking()
                .Include(r => r.EmployeeProfile)
                .Include(r => r.LeaveType)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<LeaveRequest>> GetLeaveRequestsByManagerId(int managerId)
        {
            return await _context.LeaveRequests
                .AsNoTracking()
                .Where(r => r.EmployeeProfile != null &&
                            r.EmployeeProfile.ManagerId == managerId)
                .Include(r => r.EmployeeProfile)
                .Include(r => r.LeaveType)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }
        public async Task<List<LeaveRequest>> GetLeaveRequestsByUserId(int userId)
        {
            return await _context.LeaveRequests
                .AsNoTracking()
                .Where(r => r.EmployeeProfile.UserId == userId)
                .Include(r => r.EmployeeProfile)
                .Include(r => r.LeaveType)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<LeaveBalance>> GetLeaveBalancesByUserId(int userId)
        {
            var employeeProfileId = await _context.EmployeeProfiles
                .Where(e => e.UserId == userId)
                .Select(e => e.Id)
                .FirstOrDefaultAsync();

            if (employeeProfileId == 0)
            {
                return new List<LeaveBalance>();
            }

            return await _context.LeaveBalances
                .AsNoTracking()
                .Where(b => b.EmployeeProfileId == employeeProfileId)
                .ToListAsync();
        }


        public async Task<bool> IsLeaveRequestUnderManager( int leaveRequestId, int managerProfileId)
        {
            return await _context.LeaveRequests
                .AnyAsync(r =>
                    r.Id == leaveRequestId &&
                    r.EmployeeProfile != null &&
                    r.EmployeeProfile.ManagerId == managerProfileId);
        }

    }
}
