namespace AttendanceAPI.Models
{
    public class RegisterRequest
    {
        public int Id { get; set; } // optional: will usually be ignored by EF on insert
        public Guid? UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // "manager" or "employee"
    }
}
