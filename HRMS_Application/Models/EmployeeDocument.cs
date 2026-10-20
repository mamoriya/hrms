
namespace HRMS_Application.Models
{
    public class EmployeeDocument
    {
        public int Id { get; set; }

        public int EmployeeProfileId { get; set; }

        public string FileName { get; set; } = string.Empty;

        public string FileType { get; set; } = string.Empty;

        public byte[] FileData { get; set; } = Array.Empty<byte>();

        public long FileSize { get; set; }

        public DateTime UploadedAt { get; set; }
    }
}
