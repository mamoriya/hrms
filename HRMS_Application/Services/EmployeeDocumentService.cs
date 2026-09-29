using HRMS_Application.Models;
using HRMS_Application.Repositories;

namespace HRMS_Application.Services
{
    public class EmployeeDocumentService
    {
        private readonly EmployeeDocumentRepository _repository;
        private readonly EmployeeRepository _employeeRepository;

        public EmployeeDocumentService(
            EmployeeDocumentRepository repository,
            EmployeeRepository employeeRepository)
        {
            _repository = repository;
            _employeeRepository = employeeRepository;
        }

        public async Task<EmployeeDocument?> AddDocument(
            EmployeeDocument document)
        {
            if (document == null)
            {
                return null;
            }

            if (document.EmployeeProfileId <= 0)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(document.FileName))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(document.FileType))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(document.FilePath))
            {
                return null;
            }

            if (document.FileSize <= 0)
            {
                return null;
            }

            var allowedFileTypes = new[] { "pdf", "jpg", "jpeg", "png" };

            if (!allowedFileTypes.Contains(
                    document.FileType.ToLower()))
            {
                return null;
            }

            const long maxFileSize = 5 * 1024 * 1024;

            if (document.FileSize > maxFileSize)
            {
                return null;
            }

            if (!await _repository.EmployeeExists(
                    document.EmployeeProfileId))
            {
                return null;
            }

            document.UploadedAt = DateTime.Now;

            return await _repository.AddDocument(document);
        }

        public async Task<List<EmployeeDocument>> GetDocumentsByEmployeeId(
            int employeeId)
        {
            if (employeeId <= 0)
            {
                return new List<EmployeeDocument>();
            }

            if (!await _repository.EmployeeExists(employeeId))
            {
                return new List<EmployeeDocument>();
            }

            return await _repository.GetDocumentsByEmployeeId(employeeId);
        }

        public async Task<EmployeeDocument?> GetDocumentById(
            int employeeId,
            int documentId)
        {
            if (employeeId <= 0 || documentId <= 0)
            {
                return null;
            }

            return await _repository.GetDocumentById(
                employeeId,
                documentId);
        }

        public async Task<bool> DeleteDocument(
            int employeeId,
            int documentId)
        {
            var document = await _repository.GetDocumentById(
                employeeId,
                documentId);

            if (document == null)
            {
                return false;
            }

            await _repository.DeleteDocument(document);

            return true;
        }


        public async Task<bool> IsEmployeeOwner(int employeeId,int userId)
        {
            var employee = await _employeeRepository.GetEmployeeById(employeeId);

            if (employee == null)
            {
                return false;
            }

            return employee.UserId == userId;
        }

    }
}
