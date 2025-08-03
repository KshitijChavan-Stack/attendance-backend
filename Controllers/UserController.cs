using AttendanceAPI.Models;
using AttendanceAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AttendanceDbContext _context;

        public UserController(AttendanceDbContext context)
        {
            _context = context;
        }

        // Register a new user
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest request)
        {
            if (request.Role.ToLower() == "manager")
            {
                if (await _context.Managers.AnyAsync(m => m.Email == request.Email))
                    return Conflict("Manager with this email already exists.");

                var manager = new Manager
                {
                    Name = request.Name,
                    Email = request.Email,
                    Password = request.Password
                };

                _context.Managers.Add(manager);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Manager registered",
                    data = new { manager.Name, manager.Email }
                });
            }
            else if (request.Role.ToLower() == "employee")
            {
                if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                    return Conflict("User with this email already exists.");

                var user = new user
                {
                    Name = request.Name,
                    Email = request.Email,
                    Password = request.Password
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Employee registered",
                    data = new { user.Name, user.Email }
                });
            }
            else
            {
                return BadRequest("Invalid role. Must be 'manager' or 'employee'.");
            }
        }

        // Get all users
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        // Get user by email (since ID is being removed)
        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return NotFound("User not found.");

            return Ok(user);
        }

        // Login (email + password only)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] user login)
        {
            var manager = await _context.Managers.FirstOrDefaultAsync(m => m.Email == login.Email);

            if (manager != null && manager.Password == login.Password)
            {
                return Ok(new
                {
                    message = "Manager login successful",
                    isManager = true,
                    user = new { manager.Name, manager.Email }
                });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == login.Email);

            if (user == null || user.Password != login.Password)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            return Ok(new
            {
                message = "Login successful",
                isManager = false,
                user = new { user.Name, user.Email }
            });
        }
    }
}
