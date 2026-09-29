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
        public async Task<IActionResult> AddDocument(
            int id,
            EmployeeDocument document)
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

            document.EmployeeProfileId = id;

            var result = await _service.AddDocument(document);

            if (result == null)
            {
                return BadRequest("Invalid document data.");
            }

            return Ok(result);
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
        public async Task<IActionResult> GetDocument(
            int id,
            int documentId)
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

            var document =
                await _service.GetDocumentById(id, documentId);

            if (document == null)
            {
                return NotFound("Document not found.");
            }

            return Ok(document);
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