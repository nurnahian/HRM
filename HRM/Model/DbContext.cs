using Microsoft.EntityFrameworkCore;

namespace HRM.Model
{
    public class AppDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeAttendance> EmployeeAttendance { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        internal object Entry(string employeeName, string employeeCode)
        {
            throw new NotImplementedException();
        }
    }

}
