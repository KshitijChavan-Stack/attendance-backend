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

        // Helper method to validate user credentials (checks both Users and Managers tables)
        private async Task<(bool isValid, string userType, string name)> ValidateUserCredentials(string email, string password)
        {
            // Check if user exists in Users table
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
            if (user != null)
                return (true, "employee", user.Name);

            // Check if user exists in Managers table
            var manager = await _context.Managers.FirstOrDefaultAsync(m => m.Email == email && m.Password == password);
            if (manager != null)
                return (true, "manager", manager.Name);

            return (false, null, null);
        }

        [HttpPost("entry")]
        public async Task<IActionResult> MarkEntry([FromBody] Attendance model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                return BadRequest("Email and Password are required.");

            // Validate credentials against both Users and Managers tables
            var (isValid, userType, userName) = await ValidateUserCredentials(model.Email, model.Password);
            if (!isValid)
                return Unauthorized("Invalid credentials.");

            // Set the employee name based on the validated user
            model.EmployeeName = userName;
            model.EntryTime = DateTime.UtcNow;
            model.ExitTime = null;

            _context.Attendances.Add(model);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Entry marked",
                userType = userType,
                data = model
            });
        }

        [HttpPost("exit")]
        public async Task<IActionResult> MarkExit([FromBody] Attendance model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                return BadRequest("Email and Password are required.");

            // Validate credentials against both Users and Managers tables
            var (isValid, userType, userName) = await ValidateUserCredentials(model.Email, model.Password);
            if (!isValid)
                return Unauthorized("Invalid credentials.");

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
                userType = userType,
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

        // NEW ENDPOINT: Get all employees and managers for manager dashboard
        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var employees = await _context.Users
                .Select(u => new {
                    Email = u.Email,
                    Name = u.Name,
                    Role = "Employee"
                })
                .ToListAsync();

            var managers = await _context.Managers
                .Select(m => new {
                    Email = m.Email,
                    Name = m.Name,
                    Role = "Manager"
                })
                .ToListAsync();

            var allUsers = employees.Concat(managers).ToList();

            return Ok(allUsers);
        }

        // NEW ENDPOINT: Check user type for login redirection
        [HttpPost("check-user-type")]
        public async Task<IActionResult> CheckUserType([FromBody] Attendance model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                return BadRequest("Email and Password are required.");

            try
            {
                // Check if user exists in Users table
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);
                if (user != null)
                {
                    return Ok(new
                    {
                        userType = "employee",
                        name = user.Name,
                        email = user.Email
                    });
                }

                // Check if user exists in Managers table
                var manager = await _context.Managers.FirstOrDefaultAsync(m => m.Email == model.Email && m.Password == model.Password);
                if (manager != null)
                {
                    return Ok(new
                    {
                        userType = "manager",
                        name = manager.Name,
                        email = manager.Email
                    });
                }

                return Unauthorized("Invalid credentials.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}