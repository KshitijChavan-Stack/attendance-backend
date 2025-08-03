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
            var connectionString = "Host=dpg-d25ih1re5dus73a52jjg-a;Port=5432;Database=office_logger_db;Username=office_logger_db_user;Password=bkVO99om3XZuCEOa4H85lX18cy4xHC1r";

            optionsBuilder.UseNpgsql(connectionString);

            return new AttendanceDbContext(optionsBuilder.Options);
        }
    }
}
