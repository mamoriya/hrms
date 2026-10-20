using HRMS_Application.Models;
using HRMS_Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRMS_Application.Controllers
{
    [Route("api/leave-requests")]
    [ApiController]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly LeaveRequestService _service;

        public LeaveRequestsController(LeaveRequestService service)
        {
            _service = service;
        }


        [Authorize(Roles = "Admin,Manager")]
        [HttpGet]
        public async Task<IActionResult> GetAllLeaveRequests()
        {
            var result = await _service.GetAllLeaveRequests();

            return Ok(result);
        }


        [Authorize(Roles = "Manager")]
        [HttpGet("my-team")]
        public async Task<IActionResult> GetMyTeamLeaveRequests()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out var userId))
            {
                return Unauthorized("Invalid user identity.");
            }

            var managerProfileId =
                await _service.GetEmployeeProfileIdByUserId(userId);

            if (managerProfileId == null)
            {
                return NotFound("Manager profile not found.");
            }

            var result = await _service.GetLeaveRequestsByManagerId(
                managerProfileId.Value);

            return Ok(result);
        }

        [Authorize(Roles = "Employee")]
        [HttpPost]
        public async Task<IActionResult> CreateLeaveRequest(
            LeaveRequest leaveRequest)
        {
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out var userId))
            {
                return Unauthorized("Invalid user identity.");
            }

            var employeeProfileId =
                await _service.GetEmployeeProfileIdByUserId(userId);

            if (employeeProfileId == null)
            {
                return BadRequest(
                    "Employee profile not found for the logged-in user.");
            }

            leaveRequest.EmployeeProfileId = employeeProfileId.Value;

           
            try
            {
                var result = await _service.CreateLeaveRequest(leaveRequest);

                return Ok(result);
            }
            catch (LeaveRequestValidationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }

        }



        [Authorize(Roles = "Admin,Manager")]
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveLeaveRequest(int id)
        {
            if (User.IsInRole("Manager"))
            {
                var userIdValue = User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

                if (!int.TryParse(userIdValue, out var userId))
                {
                    return Unauthorized("Invalid user identity.");
                }

                var managerProfileId =
                    await _service.GetEmployeeProfileIdByUserId(userId);

                if (managerProfileId == null)
                {
                    return NotFound("Manager profile not found.");
                }

                var isUnderManager =
                    await _service.IsLeaveRequestUnderManager(
                        id, managerProfileId.Value);

                if (!isUnderManager)
                {
                    return Forbid();
                }
            }

            var result = await _service.ApproveLeaveRequest(id);

            if (result == null)
            {
                return BadRequest("Leave request cannot be approved.");
            }

            return Ok(result);
        }



        [Authorize(Roles = "Admin,Manager")]
        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectLeaveRequest(
            int id,
            [FromBody] string rejectionReason)
        {
            if (User.IsInRole("Manager"))
            {
                var userIdValue = User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

                if (!int.TryParse(userIdValue, out var userId))
                {
                    return Unauthorized("Invalid user identity.");
                }

                var managerProfileId =
                    await _service.GetEmployeeProfileIdByUserId(userId);

                if (managerProfileId == null)
                {
                    return NotFound("Manager profile not found.");
                }

                var isUnderManager =
                    await _service.IsLeaveRequestUnderManager(
                        id, managerProfileId.Value);

                if (!isUnderManager)
                {
                    return Forbid();
                }
            }

            var result = await _service.RejectLeaveRequest(
                id, rejectionReason);

            if (result == null)
            {
                return BadRequest("Leave request cannot be rejected.");
            }

            return Ok(result);
        }


        [Authorize(Roles = "Employee")]
        [HttpGet("my-requests")]
        public async Task<IActionResult> GetMyLeaveRequests()
        {
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out var userId))
            {
                return Unauthorized("Invalid user identity.");
            }

            var result = await _service.GetLeaveRequestsByUserId(userId);

            return Ok(result);
        }


        [Authorize(Roles = "Employee")]
        [HttpGet("my-balances")]
        public async Task<IActionResult> GetMyLeaveBalances()
        {
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out var userId))
            {
                return Unauthorized("Invalid user identity.");
            }

            var result = await _service.GetLeaveBalancesByUserId(userId);

            return Ok(result);
        }
    }
}
