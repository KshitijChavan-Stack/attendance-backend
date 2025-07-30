using AttendanceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AttendanceAPI.Data
{
    public class AttendanceDbContext : DbContext
    {
        public AttendanceDbContext(DbContextOptions<AttendanceDbContext> options)
       : base(options)
        {
        }

        public DbSet<user> Users { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Manager> Managers { get; set; }

    }
}
