using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRM.Model
{
    public class Employee
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int employeeId { get; set; }
        public string employeeName { get; set; }
        public string employeeCode { get; set; }
        public decimal employeeSalary { get; set; }
        public int supervisorId { get; set; }
    }

}
