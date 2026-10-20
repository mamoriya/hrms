using HRMS_Application.Models;
using HRMS_Application.Repositories;

namespace HRMS_Application.Services
{
    public class LeaveRequestService
    {
        private readonly LeaveRequestRepository _repository;

        public LeaveRequestService(LeaveRequestRepository repository)
        {
            _repository = repository;
        }


        public async Task<int?> GetEmployeeProfileIdByUserId(int userId)
        {
            return await _repository.GetEmployeeProfileIdByUserId(userId);
        }

        public async Task<LeaveRequest?> CreateLeaveRequest(LeaveRequest leaveRequest)
        {
            if (leaveRequest == null)
            {
                throw new LeaveRequestValidationException(
            "Leave request data is missing.");
            }

            if (leaveRequest.EmployeeProfileId <= 0)
            {
                throw new LeaveRequestValidationException(
            "Invalid employee profile.");
            }

            if (leaveRequest.LeaveTypeId <= 0)
            {
                throw new LeaveRequestValidationException(
             "Please select a valid leave type.");
            }

            if (leaveRequest.EndDate < leaveRequest.StartDate)
            {
                throw new LeaveRequestValidationException(
             "End date cannot be earlier than start date.");
            }

            if (!await _repository.EmployeeExists(
                    leaveRequest.EmployeeProfileId))
            {
                throw new LeaveRequestValidationException(
            "Employee profile does not exist.");
            }

            if (!await _repository.LeaveTypeExists(
                    leaveRequest.LeaveTypeId))
            {
                throw new LeaveRequestValidationException(
             "Selected leave type does not exist.");
            }

            var balance = await _repository.GetLeaveBalance(
                leaveRequest.EmployeeProfileId,
                leaveRequest.LeaveTypeId);

            if (balance == null)
            {
                throw new LeaveRequestValidationException(
            "No leave balance found for the selected leave type.");
            }

            var requestedDays = CalculateLeaveDays(
                leaveRequest.StartDate,
                leaveRequest.EndDate);

            if (requestedDays <= 0)
            {
                throw new LeaveRequestValidationException(
           "The selected date range contains no working days.");
            }

            if (requestedDays > balance.Remaining)
            {
                throw new LeaveRequestValidationException(
           $"Insufficient leave balance. Requested: {requestedDays} day(s), " +
           $"Remaining: {balance.Remaining} day(s).");
            }

            var overlappingRequests =
                await _repository.GetOverlappingRequests(
                    leaveRequest.EmployeeProfileId,
                    leaveRequest.StartDate,
                    leaveRequest.EndDate);

            if (overlappingRequests.Any())
            {
                throw new LeaveRequestValidationException(
            "A pending or approved leave request already exists " +
            "for the selected dates.");
            }

            balance.Reserved += requestedDays;

            leaveRequest.Status = "PENDING";
            leaveRequest.CreatedAt = DateTime.Now;

            await _repository.UpdateLeaveBalance(balance);

            return await _repository.CreateLeaveRequestWithBalance(leaveRequest, balance);
        }

        public async Task<LeaveRequest?> ApproveLeaveRequest(int id)
        {
            var leaveRequest = await _repository.GetLeaveRequestById(id);

            if (leaveRequest == null)
            {
                return null;
            }

            if (leaveRequest.Status != "PENDING")
            {
                return null;
            }

            var requestedDays = CalculateLeaveDays(
                leaveRequest.StartDate,
                leaveRequest.EndDate);

            var balance = await _repository.GetLeaveBalance(
                leaveRequest.EmployeeProfileId,
                leaveRequest.LeaveTypeId);

            if (balance == null)
            {
                return null;
            }

            balance.Reserved -= requestedDays;
            balance.Used += requestedDays;

            leaveRequest.Status = "APPROVED";

            return await _repository.UpdateLeaveRequestWithBalance(leaveRequest, balance);
        }

        public async Task<LeaveRequest?> RejectLeaveRequest(int id, string rejectionReason)
        {
            var leaveRequest = await _repository.GetLeaveRequestById(id);

            if (leaveRequest == null)
            {
                return null;
            }

            if (leaveRequest.Status != "PENDING")
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(rejectionReason))
            {
                return null;
            }

            var requestedDays = CalculateLeaveDays(
                leaveRequest.StartDate,
                leaveRequest.EndDate);

            var balance = await _repository.GetLeaveBalance(
                leaveRequest.EmployeeProfileId,
                leaveRequest.LeaveTypeId);

            if (balance == null)
            {
                return null;
            }

            balance.Reserved -= requestedDays;

            leaveRequest.Status = "REJECTED";
            leaveRequest.RejectionReason = rejectionReason;

            return await _repository.UpdateLeaveRequestWithBalance(leaveRequest, balance);
        }
        private int CalculateLeaveDays(DateTime startDate, DateTime endDate)
        {
            int days = 0;

            for (var date = startDate.Date;
                 date <= endDate.Date;
                 date = date.AddDays(1))
            {
                // Saturday and Sunday are excluded.
                if (date.DayOfWeek != DayOfWeek.Saturday &&
                    date.DayOfWeek != DayOfWeek.Sunday)
                {
                    days++;
                }
            }

            return days;
        }



        public async Task<List<LeaveRequest>> GetAllLeaveRequests()
        {
            return await _repository.GetAllLeaveRequests();
        }


        public async Task<List<LeaveRequest>> GetLeaveRequestsByManagerId(
            int managerId)
        {
            return await _repository.GetLeaveRequestsByManagerId(managerId);
        }

        public async Task<List<LeaveRequest>> GetLeaveRequestsByUserId(int userId)
        {
            return await _repository.GetLeaveRequestsByUserId(userId);
        }

        public async Task<List<LeaveBalance>> GetLeaveBalancesByUserId(int userId)
        {
            return await _repository.GetLeaveBalancesByUserId(userId);
        }

        public async Task<bool> IsLeaveRequestUnderManager(int leaveRequestId, int managerProfileId)
        {
            return await _repository.IsLeaveRequestUnderManager(
                leaveRequestId,
                managerProfileId);
        }

    }
}
