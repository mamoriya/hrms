using HRMS_Application.Models;
using HRMS_Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        public async Task<IActionResult> AddDocument(
            int id,
            EmployeeDocument document)
        {
            document.EmployeeProfileId = id;

            var result = await _service.AddDocument(document);

            if (result == null)
            {
                return BadRequest("Invalid document data.");
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetDocuments(int id)
        {
            var documents =
                await _service.GetDocumentsByEmployeeId(id);

            return Ok(documents);
        }

        [HttpGet("{documentId}")]
        public async Task<IActionResult> GetDocument(
            int id,
            int documentId)
        {
            var document =
                await _service.GetDocumentById(id, documentId);

            if (document == null)
            {
                return NotFound("Document not found.");
            }

            return Ok(document);
        }

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
