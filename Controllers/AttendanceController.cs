using AttendanceAPI.Models;
using AttendanceAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly AttendanceDbContext _context;

        public AttendanceController(AttendanceDbContext context)
        {
            _context = context;
        }

        [HttpPost("entry")]
        public async Task<IActionResult> MarkEntry([FromBody] Attendance model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                return BadRequest("Email and Password are required.");

            // Optional: Validate user exists
            var userExists = await _context.Users.AnyAsync(u => u.Email == model.Email && u.Password == model.Password);
            if (!userExists)
                return Unauthorized("Invalid credentials.");

            model.EntryTime = DateTime.UtcNow;
            model.ExitTime = null;

            _context.Attendances.Add(model);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Entry marked", data = model });
        }

        [HttpPost("exit")]
        public async Task<IActionResult> MarkExit([FromBody] Attendance model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                return BadRequest("Email and Password are required.");

            var record = await _context.Attendances
                .Where(a => a.Email == model.Email && a.ExitTime == null)
                .OrderByDescending(a => a.EntryTime)
                .FirstOrDefaultAsync();

            if (record == null)
                return NotFound("No active entry found for this user or already exited.");

            record.ExitTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            TimeSpan duration = record.ExitTime.Value - record.EntryTime.Value;

            return Ok(new
            {
                message = "Exit marked",
                entryTime = record.EntryTime,
                exitTime = record.ExitTime,
                duration = duration.ToString(@"hh\:mm\:ss")
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var records = await _context.Attendances.ToListAsync();
            return Ok(records);
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var records = await _context.Attendances
                .Where(a => a.Email == email)
                .ToListAsync();

            if (!records.Any())
                return NotFound("No records found for this employee.");

            return Ok(records);
        }

        [HttpGet("{email}/monthly")]
        public async Task<IActionResult> GetMonthlyData(string email, [FromQuery] int month, [FromQuery] int year)
        {
            var records = await _context.Attendances
                .Where(a => a.Email == email &&
                            a.EntryTime.HasValue &&
                            a.EntryTime.Value.Month == month &&
                            a.EntryTime.Value.Year == year)
                .ToListAsync();

            if (!records.Any())
                return NotFound("No records found for this user in the specified month and year.");

            var presentDays = records
                .Select(a => a.EntryTime.Value.Date)
                .Distinct()
                .Count();

            var totalHoursWorked = records
                .Where(a => a.EntryTime.HasValue && a.ExitTime.HasValue)
                .Select(a => (a.ExitTime.Value - a.EntryTime.Value).TotalHours)
                .Sum();

            return Ok(new
            {
                records,
                presentDays,
                totalHoursWorked = Math.Round(totalHoursWorked, 2)
            });
        }
    }
}
