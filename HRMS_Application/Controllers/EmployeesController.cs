using HRMS_Application.Models;
using HRMS_Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRMS_Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeService _service;

        public EmployeesController(EmployeeService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateEmployee(EmployeeProfile employee)
        {
            var result = await _service.CreateEmployee(employee);

            if (result == null)
            {
                return BadRequest("Invalid employee data.");
            }

            return Ok(result);
        }

        [Authorize(Roles = "Admin,Manager,Employee")]
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _service.GetAllEmployees();

            return Ok(employees);
        }

        [Authorize(Roles = "Admin,Employee")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee( int id, EmployeeProfile employee)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var currentUserRole = User.FindFirstValue(
                ClaimTypes.Role
            );

            if (currentUserRole == "Employee")
            {
                var isOwner = await _service.IsEmployeeOwner(
                    id,
                    currentUserId
                );

                if (!isOwner)
                {
                    return Forbid();
                }
            }

            var result = await _service.UpdateEmployee(id, employee);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
