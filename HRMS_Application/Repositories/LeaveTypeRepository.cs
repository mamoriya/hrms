using HRMS_Application.Data;
using HRMS_Application.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS_Application.Repositories
{
    public class LeaveTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public LeaveTypeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> NameExists(string name)
        {
            return await _context.LeaveTypes
                .AnyAsync(l => l.Name == name);
        }

        public async Task<LeaveType> CreateLeaveType(LeaveType leaveType)
        {
            _context.LeaveTypes.Add(leaveType);

            await _context.SaveChangesAsync();

            return leaveType;
        }

        public async Task<List<LeaveType>> GetAllLeaveTypes()
        {
            return await _context.LeaveTypes
                .ToListAsync();
        }

        public async Task<LeaveType?> GetLeaveTypeById(int id)
        {
            return await _context.LeaveTypes
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<LeaveType> UpdateLeaveType(LeaveType leaveType)
        {
            _context.LeaveTypes.Update(leaveType);

            await _context.SaveChangesAsync();

            return leaveType;
        }

        public async Task DeleteLeaveType(LeaveType leaveType)
        {
            _context.LeaveTypes.Remove(leaveType);

            await _context.SaveChangesAsync();
        }
    }
}
