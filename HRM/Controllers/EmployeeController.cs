using HRM.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace HRM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }

        // GET api/employees
        [HttpGet("Details")]
        public IActionResult GetEmployees()
        {
            var employeesDetails = _context.Employees.ToList();
            return Ok(employeesDetails);
        }

        // GET api/employees/{id}
        [HttpGet("Details{id}")]
        public IActionResult GetEmployee(int id)
        {
            var employee = _context.Employees.Find(id);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        // POST api/employees
        [HttpPost("Add")]
        public IActionResult CreateEmployee(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Employees.Add(employee);
                _context.SaveChanges();
                return CreatedAtAction(nameof(GetEmployee), new { id = employee.employeeId }, employee);
            }

            return BadRequest(ModelState);
        }

        // PUT api/employees/{id}
        [HttpPut("update{id}")]
        public IActionResult UpdateEmployee(int id, Employee updatedEmployee)
        {
            if (id != updatedEmployee.employeeId)
            {
                return BadRequest();
            }

            _context.Entry(updatedEmployee).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Employees.Any(e => e.employeeId == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE api/employees/{id}
        [HttpDelete("delete{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            var employee = _context.Employees.Find(id);

            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPut("{employeeId}")]
        public IActionResult UpdatedEmployee(int employeeId, Employee updatedEmployee)
        {
            var existingEmployee = _context.Employees.Find(employeeId);

            if (existingEmployee == null)
            {
                return NotFound();
            }

            // Check for duplicate employee code
            if (_context.Employees.Any(e => e.employeeCode == updatedEmployee.employeeCode && e.employeeId != employeeId))
            {
                return BadRequest("Employee code already exists.");
            }

            existingEmployee.employeeName = updatedEmployee.employeeName;
            existingEmployee.employeeCode = updatedEmployee.employeeCode;

            _context.SaveChanges();

            return Ok(existingEmployee);
        }


        [HttpGet("3rd-highest-salary")]
        public IActionResult GetEmployeeWithThirdHighestSalary()
        {
            var employeesWithSalaries = _context.Employees
                .OrderByDescending(e => e.employeeSalary)
                .Select(e => new
                {
                    e.employeeName,
                    e.employeeSalary
                })
                .ToList();

            if (employeesWithSalaries.Count < 3)
            {
                return NotFound("There are not enough employees in the database to determine the 3rd highest salary.");
            }

            var thirdHighestSalaryEmployee = employeesWithSalaries[2];

            return Ok(thirdHighestSalaryEmployee);
        }

        [HttpGet("no-absence-salary")]
        public IActionResult GetEmployeesWithNoAbsenceSortedBySalary()
        {
            var employeesWithNoAbsence = _context.Employees
                .Where(e => _context.EmployeeAttendance.Any(a => a.EmployeeId == e.employeeId && a.IsAbsent == true))
                .OrderByDescending(e => e.employeeSalary)
                .ToList();

            if (employeesWithNoAbsence.Count == 0)
            {
                return NotFound("No employees with no absence records found.");
            }

            return Ok(employeesWithNoAbsence);
        }

        [HttpGet("monthly-attendance-report")]
        public IActionResult GetMonthlyAttendanceReport()
        {
            var monthlyAttendanceReport = _context.Employees
                .Select(e => new
                {
                    EmployeeName = e.employeeName,
                    MonthName = DateTime.Now.ToString("MMMM"),
                    PayableSalary = e.employeeSalary,
                    TotalPresent = _context.EmployeeAttendance
                        .Where(a => a.EmployeeId == e.employeeId && a.IsPresent == true)
                        .Count(),
                    TotalAbsent = _context.EmployeeAttendance
                        .Where(a => a.EmployeeId == e.employeeId && a.IsAbsent == true)
                        .Count(),
                    TotalOffday = _context.EmployeeAttendance
                        .Where(a => a.EmployeeId == e.employeeId && a.IsOffday == true)
                        .Count()
                })
                .ToList();

            return Ok(monthlyAttendanceReport);
        }



        // GET api/employees/hierarchy/{employeeId}
        [HttpGet("hierarchy/{employeeId}")]
        public IActionResult GetEmployeeHierarchy(int employeeId)
        {
            var hierarchy = new List<string>();
            var employee = _context.Employees.FirstOrDefault(e => e.employeeId == employeeId);

            if (employee == null)
            {
                return NotFound("Employee not found.");
            }
            
            while (employee != null)
            {

                var hasDuplicates = hierarchy.GroupBy(x => x).Any(g => g.Count() >1);
                if(hasDuplicates == true)
                {
                    break;
                }
                else
                {
                    hierarchy.Add(employee.employeeName);
                    employee = _context.Employees.FirstOrDefault(e => e.employeeId == employee.supervisorId);
                }
                
            }
           

            hierarchy.Reverse(); // Reverse the list to get the hierarchy from top to bottom

            var hierarchyString = string.Join(" -> ", hierarchy);
            
            return Ok(hierarchyString);
        }

    }

}
