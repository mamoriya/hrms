
namespace HRMS_Application.Services
{
    public class LeaveRequestValidationException : Exception
    {
        public LeaveRequestValidationException(string message)
            : base(message)
        {
        }
    }
}
