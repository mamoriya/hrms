using HRMS_Application.Models;
using HRMS_Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Application.Controllers
{
    [Route("api/leave-types")]
    [ApiController]
    public class LeaveTypesController : ControllerBase
    {
        private readonly LeaveTypeService _service;

        public LeaveTypesController(LeaveTypeService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateLeaveType(
            LeaveType leaveType)
        {
            var result = await _service.CreateLeaveType(leaveType);

            if (result == null)
            {
                return BadRequest("Invalid leave type data.");
            }

            return Ok(result);
        }

        [Authorize(Roles = "Admin,Manager,Employee")]
        [HttpGet]
        public async Task<IActionResult> GetAllLeaveTypes()
        {
            var leaveTypes = await _service.GetAllLeaveTypes();

            return Ok(leaveTypes);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLeaveType(
            int id,
            LeaveType leaveType)
        {
            var result =
                await _service.UpdateLeaveType(id, leaveType);

            if (result == null)
            {
                return NotFound("Leave type not found.");
            }

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeaveType(int id)
        {
            var result = await _service.DeleteLeaveType(id);

            if (!result)
            {
                return NotFound("Leave type not found.");
            }

            return Ok("Leave type deleted successfully.");
        }
    }
}
