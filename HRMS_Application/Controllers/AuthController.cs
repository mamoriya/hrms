using HRMS_Application.DTOs;
using HRMS_Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Application.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _service;

        public AuthController(AuthService service)
        {
            _service = service;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _service.Login(request);

            if (result == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            return Ok(result);
        }

        [HttpPost("setup-password")]
        public async Task<IActionResult> SetupPassword(
            SetupPasswordRequest request)
        {
            var result = await _service.SetupPassword(request);

            if (result == null)
            {
                return BadRequest("Unable to setup password.");
            }

            return Ok(result);
        }
    }
}
