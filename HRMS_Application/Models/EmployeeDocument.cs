namespace HRMS_Application.Models
{
    public class EmployeeDocument
    {
        public int Id { get; set; }

        public int EmployeeProfileId { get; set; }

        public string FileName { get; set; }

        public string FileType { get; set; }

        public string FilePath { get; set; }

        public long FileSize { get; set; }

        public DateTime UploadedAt { get; set; }
    }
}
