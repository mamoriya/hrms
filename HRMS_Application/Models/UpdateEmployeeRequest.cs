namespace HRMS_Application.Models
{
    public class UpdateEmployeeRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }

        public string JobTitle { get; set; }

        public int DepartmentId { get; set; }
        public int? ManagerId { get; set; }

        public DateTime JoiningDate { get; set; }

        public string Role { get; set; }
    }
}
