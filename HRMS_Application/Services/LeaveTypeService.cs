using HRMS_Application.Models;
using HRMS_Application.Repositories;

namespace HRMS_Application.Services
{
    public class LeaveTypeService
    {
        private readonly LeaveTypeRepository _repository;

        public LeaveTypeService(LeaveTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<LeaveType?> CreateLeaveType(LeaveType leaveType)
        {
            if (leaveType == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(leaveType.Name))
            {
                return null;
            }

            if (leaveType.AnnualQuota <= 0)
            {
                return null;
            }

            if (await _repository.NameExists(leaveType.Name))
            {
                return null;
            }

            return await _repository.CreateLeaveType(leaveType);
        }

        public async Task<List<LeaveType>> GetAllLeaveTypes()
        {
            return await _repository.GetAllLeaveTypes();
        }

        public async Task<LeaveType?> UpdateLeaveType(
            int id,
            LeaveType leaveType)
        {
            var existingLeaveType =
                await _repository.GetLeaveTypeById(id);

            if (existingLeaveType == null)
            {
                return null;
            }

            if (leaveType == null)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(leaveType.Name))
            {
                return null;
            }

            if (leaveType.AnnualQuota <= 0)
            {
                return null;
            }

            existingLeaveType.Name = leaveType.Name;
            existingLeaveType.AnnualQuota = leaveType.AnnualQuota;
            existingLeaveType.CarryForward = leaveType.CarryForward;

            return await _repository.UpdateLeaveType(existingLeaveType);
        }

        public async Task<bool> DeleteLeaveType(int id)
        {
            var leaveType =
                await _repository.GetLeaveTypeById(id);

            if (leaveType == null)
            {
                return false;
            }

            await _repository.DeleteLeaveType(leaveType);

            return true;
        }
    }
}
