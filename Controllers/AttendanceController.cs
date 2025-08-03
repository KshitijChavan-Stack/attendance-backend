using AttendanceAPI.Models;
using AttendanceAPI.Data;
using Microsoft.AspNetCore.Http;
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
            model.EntryTime = DateTime.UtcNow; // Use UTC
            model.ExitTime = null;
            _context.Attendances.Add(model);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Entry marked", data = model });
        }

        [HttpPost("exit")]
        public async Task<IActionResult> MarkExit([FromBody] int user_ID)
        {
            var record = await _context.Attendances
                .Where(a => a.UserId == user_ID && a.ExitTime == null)
                .OrderByDescending(a => a.EntryTime)
                .FirstOrDefaultAsync();

            if (record == null)
                return NotFound("No active entry found for this user or already exited.");

            record.ExitTime = DateTime.UtcNow; // Use UTC
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
        //Get All Users Attendance Records
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var records = await _context.Attendances.ToListAsync();
            return Ok(records);
        }
        //Get By user ID
        [HttpGet("{userID}")]
        public async Task<IActionResult> GetByEmployee(int userID)
        {
            var records = await _context.Attendances
                .Where(a => a.UserId == userID)
                .ToListAsync();

            if (!records.Any())
                return NotFound("No records found for this employee.");

            return Ok(records);
        }

        [HttpGet("{userID}/monthly")]
        public async Task<IActionResult> GetMonthlyData(int userID, [FromQuery] int month, [FromQuery] int year)
        {
            var records = await _context.Attendances
                .Where(a => a.UserId == userID &&
                            a.EntryTime.HasValue &&
                            a.EntryTime.Value.Month == month &&
                            a.EntryTime.Value.Year == year)
                .ToListAsync();

            var presentDays = records
                .Select(a => a.EntryTime.Value.Date)
                .Distinct()
                .Count();

            var totalHoursWorked = records
                .Where(a => a.EntryTime.HasValue && a.ExitTime.HasValue)
                .Select(a => (a.ExitTime.Value - a.EntryTime.Value).TotalHours)
                .Sum();

            if (!records.Any())
                return NotFound("No records found for this user in the specified month and year.");

            return Ok(new
            {
                records,
                presentDays,
                totalHoursWorked = Math.Round(totalHoursWorked, 2)
            });
        }
    }
}
