using AttendanceAPI.Data;
using AttendanceAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly AttendanceDbContext _context;

        public ManagerController(AttendanceDbContext context)
        {
            _context = context;
        }

        // GET: api/manager
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Manager>>> GetAllManagers()
        {
            return await _context.Managers.ToListAsync();
        }

        // GET: api/manager/email@example.com
        [HttpGet("{email}")]
        public async Task<ActionResult<Manager>> GetManagerByEmail(string email)
        {
            var manager = await _context.Managers.FirstOrDefaultAsync(m => m.Email == email);

            if (manager == null)
            {
                return NotFound("Manager not found.");
            }

            return Ok(manager);
        }
    }
}
