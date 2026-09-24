using HRMS_Application.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRMS_Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseTestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DatabaseTestController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> TestDatabase()
        {
            var canConnect = await _context.Database.CanConnectAsync();

            return Ok(new
            {
                DatabaseConnected = canConnect
            });
        }
    }
}
