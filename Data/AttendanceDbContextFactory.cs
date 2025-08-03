using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using AttendanceAPI.Data;

namespace AttendanceAPI.Data
{
    public class AttendanceDbContextFactory : IDesignTimeDbContextFactory<AttendanceDbContext>
    {
        public AttendanceDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AttendanceDbContext>();

            // ✅ Correct PostgreSQL connection string (no tcp://)
            var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
            optionsBuilder.UseNpgsql(connectionString);

            return new AttendanceDbContext(optionsBuilder.Options);
        }
    }
}
