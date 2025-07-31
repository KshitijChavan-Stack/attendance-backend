using AttendanceAPI.Data;
using AttendanceAPI.Models;
using Microsoft.AspNetCore.Http;
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

        // GET: api/manager/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Manager>> GetManager(int id)
        {
            var manager = await _context.Managers.FindAsync(id);

            if (manager == null)
            {
                return NotFound("Manager not found.");
            }

            return manager;
        }
    }
}
