using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AttendanceAPI.Data
{
    public class AttendanceDbContextFactory : IDesignTimeDbContextFactory<AttendanceDbContext>
    {
        public AttendanceDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AttendanceDbContext>();

            // Use your PostgreSQL connection string here
            optionsBuilder.UseNpgsql("Host=dpg-d25ih1re5dus73a52jjg-a.oregon-postgres.render.com;Port=5432;Username=office_logger_db_user;Password=bkVO99om3XZuCEOa4H85lX18cy4xHC1r;Database=office_logger_db;SSL Mode=Require;Trust Server Certificate=true");

            return new AttendanceDbContext(optionsBuilder.Options);
        }
    }
}
