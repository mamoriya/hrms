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
    }
}
