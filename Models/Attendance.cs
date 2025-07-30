namespace AttendanceAPI.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime? EntryTime { get; set; }
        public DateTime? ExitTime { get; set; }
    }

}
