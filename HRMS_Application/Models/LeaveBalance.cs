namespace HRMS_Application.Models
{
    public class LeaveBalance
    {
        public int Id { get; set; }

        public int EmployeeProfileId { get; set; }

        public int LeaveTypeId { get; set; }

        public int TotalAllotted { get; set; }

        public int Used { get; set; }

        public int Reserved { get; set; }

        public int Remaining
        {
            get
            {
                return TotalAllotted - Used - Reserved;
            }
        }
    }
}
