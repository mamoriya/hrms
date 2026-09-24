namespace HRMS_Application.Models
{
    public class EmployeeProfile
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public string JobTitle { get; set; }

        public int DepartmentId { get; set; }

        public int? ManagerId { get; set; }

        public DateTime JoiningDate { get; set; }
    }
}
