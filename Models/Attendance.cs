namespace AttendanceAPI.Models
{
    public class Attendance
    {
        public int AttendanceId { get; set; } // <-- This will be the PK
        public string Email { get; set; }            // New: Acts as identifier
        public string Password { get; set; }         // New: For verification (Not recommended to store plaintext in real apps)
        public string EmployeeName { get; set; }
        public DateTime? EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }
    }
}
