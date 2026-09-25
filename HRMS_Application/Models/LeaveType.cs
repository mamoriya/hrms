namespace HRMS_Application.Models
{
    public class LeaveType
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int AnnualQuota { get; set; }

        public bool CarryForward { get; set; }
    }
}
