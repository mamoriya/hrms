namespace HRMS_Application.Models
{
    public class LeaveRequest
    {
        public int Id { get; set; }

        public int EmployeeProfileId { get; set; }

        public int LeaveTypeId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string? Reason { get; set; }

        public string Status { get; set; }

        public string? RejectionReason { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
