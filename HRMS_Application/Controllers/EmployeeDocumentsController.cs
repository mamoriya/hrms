using HRMS_Application.Models;
using HRMS_Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRMS_Application.Controllers
{
    [Route("api/employees/{id}/documents")]
    [ApiController]
    public class EmployeeDocumentsController : ControllerBase
    {
        private readonly EmployeeDocumentService _service;

        public EmployeeDocumentsController(
            EmployeeDocumentService service)
        {
            _service = service;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!
            );
        }


        [Authorize(Roles = "Admin,Employee")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddDocument(
            int id,
            IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Please select a file.");

            const long maxFileSize = 5 * 1024 * 1024;

            if (file.Length > maxFileSize)
                return BadRequest("File size must not exceed 5 MB.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };

            if (!allowedExtensions.Contains(extension))
                return BadRequest("Only PDF, JPG, JPEG and PNG files are allowed.");

            var role = User.FindFirstValue(ClaimTypes.Role);

            if (role == "Employee")
            {
                var userId = GetCurrentUserId();
                var isOwner = await _service.IsEmployeeOwner(id, userId);

                if (!isOwner)
                    return Forbid();
            }

            // Read the actual file content
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var document = new EmployeeDocument
            {
                EmployeeProfileId = id,
                FileName = Path.GetFileName(file.FileName),
                FileType = extension.TrimStart('.'),
                FileData = memoryStream.ToArray(),
                FileSize = file.Length
            };

            var result = await _service.AddDocument(document);

            if (result == null)
                return BadRequest("Unable to upload document.");

            // Return metadata only, not the BLOB
            return Ok(new
            {
                result.Id,
                result.EmployeeProfileId,
                result.FileName,
                result.FileType,
                result.FileSize,
                result.UploadedAt
            });
        }


        [Authorize(Roles = "Admin,Employee")]
        [HttpGet]
        public async Task<IActionResult> GetDocuments(int id)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (role == "Employee")
            {
                var userId = GetCurrentUserId();

                var isOwner =
                    await _service.IsEmployeeOwner(id, userId);

                if (!isOwner)
                {
                    return Forbid();
                }
            }

            var documents =
                await _service.GetDocumentsByEmployeeId(id);

            return Ok(documents);
        }


        [Authorize(Roles = "Admin,Employee")]
        [HttpGet("{documentId}")]
        public async Task<IActionResult> GetDocument(int id, int documentId)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (role == "Employee")
            {
                var userId = GetCurrentUserId();
                var isOwner = await _service.IsEmployeeOwner(id, userId);

                if (!isOwner)
                    return Forbid();
            }

            var document = await _service.GetDocumentById(id, documentId);

            if (document == null)
                return NotFound("Document not found.");

            var contentType = document.FileType.ToLowerInvariant() switch
            {
                "pdf" => "application/pdf",
                "jpg" or "jpeg" => "image/jpeg",
                "png" => "image/png",
                _ => "application/octet-stream"
            };

            return File(
                document.FileData,
                contentType,
                document.FileName,
                enableRangeProcessing: true
            );
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{documentId}")]
        public async Task<IActionResult> DeleteDocument(
            int id,
            int documentId)
        {
            var result =
                await _service.DeleteDocument(id, documentId);

            if (!result)
            {
                return NotFound("Document not found.");
            }

            return Ok("Document deleted successfully.");
        }
    }
}