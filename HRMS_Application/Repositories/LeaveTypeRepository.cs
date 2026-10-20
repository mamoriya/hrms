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

        public async Task<LeaveType> CreateLeaveTypeWithBalances( LeaveType leaveType)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                _context.LeaveTypes.Add(leaveType);
                await _context.SaveChangesAsync();

                var employeeIds = await _context.EmployeeProfiles
                    .Select(e => e.Id)
                    .ToListAsync();

                var balances = employeeIds.Select(employeeId =>
                    new LeaveBalance
                    {
                        EmployeeProfileId = employeeId,
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

                await transaction.CommitAsync();
                return leaveType;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task CreateLeaveBalancesForAllEmployees( LeaveType leaveType)
        {
            var employeeIds = await _context.EmployeeProfiles
                .Select(e => e.Id)
                .ToListAsync();

            var existingEmployeeIds = await _context.LeaveBalances
                .Where(b => b.LeaveTypeId == leaveType.Id)
                .Select(b => b.EmployeeProfileId)
                .ToListAsync();

            var newBalances = employeeIds
                .Where(employeeId => !existingEmployeeIds.Contains(employeeId))
                .Select(employeeId => new LeaveBalance
                {
                    EmployeeProfileId = employeeId,
                    LeaveTypeId = leaveType.Id,
                    TotalAllotted = leaveType.AnnualQuota,
                    Used = 0,
                    Reserved = 0
                })
                .ToList();

            if (newBalances.Count > 0)
            {
                _context.LeaveBalances.AddRange(newBalances);
                await _context.SaveChangesAsync();
            }
        }


        public async Task UpdateLeaveBalancesForLeaveType( int leaveTypeId, int newAnnualQuota)
        {
            var balances = await _context.LeaveBalances
                .Where(b => b.LeaveTypeId == leaveTypeId)
                .ToListAsync();

            foreach (var balance in balances)
            {
                balance.TotalAllotted = newAnnualQuota;
            }

            if (balances.Count > 0)
            {
                await _context.SaveChangesAsync();
            }
        }


        public async Task<LeaveType> UpdateLeaveTypeWithBalances(
            LeaveType existingLeaveType, string newName, int newAnnualQuota, bool newCarryForward)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                existingLeaveType.Name = newName;
                existingLeaveType.AnnualQuota = newAnnualQuota;
                existingLeaveType.CarryForward = newCarryForward;

                await _context.SaveChangesAsync();

                var balances = await _context.LeaveBalances
                    .Where(b => b.LeaveTypeId == existingLeaveType.Id)
                    .ToListAsync();

                foreach (var balance in balances)
                {
                    balance.TotalAllotted = newAnnualQuota;
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return existingLeaveType;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task<bool> NameExistsExceptId( string name, int id)
        {
            return await _context.LeaveTypes.AnyAsync(
                l => l.Name == name && l.Id != id);
        }

        public async Task<bool> CanUpdateAnnualQuota( int leaveTypeId, int newAnnualQuota)
        {
            return !await _context.LeaveBalances.AnyAsync(b =>
                b.LeaveTypeId == leaveTypeId &&
                b.Used + b.Reserved > newAnnualQuota);
        }


        public async Task<bool> HasRelatedRecords(int leaveTypeId)
        {
            return await _context.LeaveBalances
                .AnyAsync(b => b.LeaveTypeId == leaveTypeId)
                ||
                await _context.LeaveRequests
                .AnyAsync(r => r.LeaveTypeId == leaveTypeId);
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
