namespace AttendanceAPI.Models
{
    public class user
    {
        public int Id { get; set; }  // Primary key - keep this
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
// This class represents a user in the system with properties for Id, Name, and Email.