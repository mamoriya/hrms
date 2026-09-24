using HRMS_Application.Models;
using HRMS_Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeService _service;

        public EmployeesController(EmployeeService service)
        {
            _service = service;
        }

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

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employees = await _service.GetAllEmployees();

            return Ok(employees);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee( int id, EmployeeProfile employee)
        {
            var result = await _service.UpdateEmployee(id, employee);

            if (result == null)
            {
                return NotFound("Employee not found.");
            }

            return Ok(result);
        }
    }
}
