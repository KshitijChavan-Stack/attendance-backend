namespace AttendanceAPI.Models
{
    public class Manager
    {
        public int Id { get; set; }          // Primary Key
        public string Name { get; set; }     // Manager’s Full Name
        public string Email { get; set; }    // Login Email
        public string Password { get; set; } 
    }
}
