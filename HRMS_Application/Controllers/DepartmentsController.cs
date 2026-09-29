using HRMS_Application.Models;
using HRMS_Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly DepartmentService _service;

        public DepartmentsController(DepartmentService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateDepartment(Department department)
        {
            var result = await _service.CreateDepartment(department);

            if (result == null)
            {
                return BadRequest("Invalid department data.");
            }

            return Ok(result);
        }


        [Authorize(Roles = "Admin,Manager,Employee")]
        [HttpGet]
        public async Task<IActionResult> GetAllDepartments()
        {
            var departments = await _service.GetAllDepartments();

            return Ok(departments);
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(
            int id,
            Department department)
        {
            var result = await _service.UpdateDepartment(id, department);

            if (result == null)
            {
                return NotFound("Department not found.");
            }

            return Ok(result);
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var result = await _service.DeleteDepartment(id);

            if (!result)
            {
                return NotFound("Department not found.");
            }

            return Ok("Department deleted successfully.");
        }
    }
}
