namespace HRMS_Application.DTOs
{
    public class LoginResponse
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string Role { get; set; }

        public string Token { get; set; }
    }
}
