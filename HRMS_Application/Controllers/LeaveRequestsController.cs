using HRMS_Application.Models;
using HRMS_Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        public async Task<IActionResult> CreateLeaveRequest(
            LeaveRequest leaveRequest)
        {
            var result = await _service.CreateLeaveRequest(leaveRequest);

            if (result == null)
                return BadRequest("Invalid leave request.");

            return Ok(result);
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveLeaveRequest(int id)
        {
            var result = await _service.ApproveLeaveRequest(id);

            if (result == null)
                return BadRequest("Leave request cannot be approved.");

            return Ok(result);
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectLeaveRequest(
            int id,
            [FromBody] string rejectionReason)
        {
            var result = await _service.RejectLeaveRequest(
                id,
                rejectionReason);

            if (result == null)
                return BadRequest("Leave request cannot be rejected.");

            return Ok(result);
        }
    }
}
