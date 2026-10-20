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

            leaveType.Name = leaveType.Name.Trim();

            return await _repository.CreateLeaveTypeWithBalances(
                leaveType);
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

            var newName = leaveType.Name.Trim();

            if (await _repository.NameExistsExceptId(newName, id))
            {
                throw new ArgumentException(
                    "A leave type with this name already exists.");
            }

            var canUpdateQuota =
                await _repository.CanUpdateAnnualQuota(
                    id,
                    leaveType.AnnualQuota);

            if (!canUpdateQuota)
            {
                throw new ArgumentException(
                    "Annual quota cannot be reduced below an employee's used and reserved leave days.");
            }

            return await _repository.UpdateLeaveTypeWithBalances(
                existingLeaveType,
                newName,
                leaveType.AnnualQuota,
                leaveType.CarryForward);
        }



        public async Task<bool> DeleteLeaveType(int id)
        {
            var leaveType = await _repository.GetLeaveTypeById(id);

            if (leaveType == null)
            {
                return false;
            }

            var hasRelatedRecords =
                await _repository.HasRelatedRecords(id);

            if (hasRelatedRecords)
            {
                throw new ArgumentException(
                    "This leave type cannot be deleted because it is used by existing leave balances or leave requests.");
            }

            await _repository.DeleteLeaveType(leaveType);

            return true;
        }

    }
}
