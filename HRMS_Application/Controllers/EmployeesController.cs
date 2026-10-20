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
        public async Task<IActionResult> CreateEmployee(CreateEmployeeRequest request)
        {
            var result = await _service.CreateEmployee(request);

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



        [Authorize(Roles = "Employee")]
        [HttpGet("my-profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out var userId))
            {
                return Unauthorized("Invalid user identity.");
            }

            var employee = await _service.GetEmployeeByUserId(userId);

            if (employee == null)
            {
                return NotFound("Employee profile not found.");
            }

            return Ok(employee);
        }


        [Authorize(Roles = "Manager")]
        [HttpGet("my-team")]
        public async Task<IActionResult> GetMyTeam()
        {
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out var userId))
            {
                return Unauthorized("Invalid user identity.");
            }

            // Find the logged-in manager's EmployeeProfile
            var manager = await _service.GetEmployeeByUserId(userId);

            if (manager == null)
            {
                return NotFound("Manager profile not found.");
            }

            // Fetch only this manager's direct reports
            var teamMembers = await _service.GetDirectReports(manager.Id);

            return Ok(teamMembers);
        }


        [Authorize(Roles = "Admin,Employee")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee( int id,UpdateEmployeeRequest request)
        {
            var currentUserId =
                int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var currentUserRole =
                User.FindFirstValue(ClaimTypes.Role);

            // Employee can update only their own profile
            if (currentUserRole == "Employee")
            {
                var isOwner =
                    await _service.IsEmployeeOwner(id, currentUserId);

                if (!isOwner)
                {
                    return Forbid();
                }

                // Employee is allowed to update only
                // PhoneNumber and Address
                var existingEmployee =
                    await _service.GetEmployeeById(id);

                if (existingEmployee == null)
                {
                    return NotFound("Employee not found.");
                }

                existingEmployee.PhoneNumber = request.PhoneNumber;
                existingEmployee.Address = request.Address;

                var result =
                    await _service.UpdateEmployeeProfile(existingEmployee);

                return Ok(result);
            }

            // Admin can update complete employee profile
            var updatedEmployee =
                await _service.UpdateEmployee(id, request);

            if (updatedEmployee == 0)
            {
                return NotFound("Employee not found.");
            }

            if (updatedEmployee == 2)
            {
                return BadRequest(
                    "Role must be either Employee or Manager."
                );
            }

            if (updatedEmployee == 3)
            {
                return BadRequest(
                    "This manager has direct reports. Reassign the employees before changing the role to Employee."
                );
            }

            return Ok("Employee updated successfully.");
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee( int id, DeleteEmployeeRequest request)
        {
            var directReports = await _service.GetDirectReports(id);

            // Manager with direct reports
            if (directReports.Count > 0)
            {
                var result = await _service.DeleteEmployeeWithNewManager(
                    id,
                    request.NewManagerId
                );

                if (result == 0)
                {
                    return NotFound("Employee not found.");
                }

                if (result == 2)
                {
                    return BadRequest("Selected new manager is invalid.");
                }

                if (result == 3)
                {
                    return BadRequest(
                        "Employee cannot be assigned as their own new manager."
                    );
                }

                if (request.NewManagerId.HasValue)
                {
                    return Ok(
                        "Manager deleted and employees reassigned successfully."
                    );
                }

                return Ok(
                    "Manager deleted and employees are now without a manager."
                );
            }

            // Normal employee / manager without direct reports
            var deleteResult = await _service.DeleteEmployee(id);

            if (deleteResult == 0)
            {
                return NotFound("Employee not found.");
            }

            return Ok("Employee deleted successfully.");
        }
    }
}
