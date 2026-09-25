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

        public async Task<LeaveRequest?> CreateLeaveRequest(
            LeaveRequest leaveRequest)
        {
            if (leaveRequest == null)
            {
                return null;
            }

            if (leaveRequest.EmployeeProfileId <= 0)
            {
                return null;
            }

            if (leaveRequest.LeaveTypeId <= 0)
            {
                return null;
            }

            if (leaveRequest.EndDate < leaveRequest.StartDate)
            {
                return null;
            }

            if (!await _repository.EmployeeExists(
                    leaveRequest.EmployeeProfileId))
            {
                return null;
            }

            if (!await _repository.LeaveTypeExists(
                    leaveRequest.LeaveTypeId))
            {
                return null;
            }

            var balance = await _repository.GetLeaveBalance(
                leaveRequest.EmployeeProfileId,
                leaveRequest.LeaveTypeId);

            if (balance == null)
            {
                return null;
            }

            var requestedDays = CalculateLeaveDays(
                leaveRequest.StartDate,
                leaveRequest.EndDate);

            if (requestedDays <= 0)
            {
                return null;
            }

            if (requestedDays > balance.Remaining)
            {
                return null;
            }

            var overlappingRequests =
                await _repository.GetOverlappingRequests(
                    leaveRequest.EmployeeProfileId,
                    leaveRequest.StartDate,
                    leaveRequest.EndDate);

            if (overlappingRequests.Any())
            {
                return null;
            }

            balance.Reserved += requestedDays;

            leaveRequest.Status = "PENDING";
            leaveRequest.CreatedAt = DateTime.Now;

            await _repository.UpdateLeaveBalance(balance);

            return await _repository.CreateLeaveRequest(leaveRequest);
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

            await _repository.UpdateLeaveBalance(balance);
            await _repository.UpdateLeaveRequest(leaveRequest);

            return leaveRequest;
        }

        public async Task<LeaveRequest?> RejectLeaveRequest(
    int id,
    string rejectionReason)
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

            await _repository.UpdateLeaveBalance(balance);
            await _repository.UpdateLeaveRequest(leaveRequest);

            return leaveRequest;
        }
        private int CalculateLeaveDays(
            DateTime startDate,
            DateTime endDate)
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
    }
}
