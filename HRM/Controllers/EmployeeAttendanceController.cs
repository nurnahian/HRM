using HRM.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeAttendanceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeeAttendanceController(AppDbContext context)
        {
            _context = context;
        }

        // GET api/employeeattendance
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeAttendance>>> GetEmployeeAttendance()
        {
            var employeeAttendance = await _context.EmployeeAttendance.ToListAsync();
            return Ok(employeeAttendance);
        }

        // GET api/employeeattendance/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeAttendance>> GetEmployeeAttendance(int id)
        {
            var employeeAttendance = await _context.EmployeeAttendance.FindAsync(id);

            if (employeeAttendance == null)
            {
                return NotFound();
            }

            return Ok(employeeAttendance);
        }

        // POST api/employeeattendance
        [HttpPost]
        public async Task<ActionResult<EmployeeAttendance>> PostEmployeeAttendance(EmployeeAttendance employeeAttendance)
        {
            _context.EmployeeAttendance.Add(employeeAttendance);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmployeeAttendance), new { id = employeeAttendance.EmployeeAttendanceId }, employeeAttendance);
        }

        // PUT api/employeeattendance/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployeeAttendance(int id, EmployeeAttendance updatedEmployeeAttendance)
        {
            if (id != updatedEmployeeAttendance.EmployeeAttendanceId)
            {
                return BadRequest();
            }

            _context.Entry(updatedEmployeeAttendance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.EmployeeAttendance.Any(e => e.EmployeeAttendanceId == id))
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

        // DELETE api/employeeattendance/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeAttendance(int id)
        {
            var employeeAttendance = await _context.EmployeeAttendance.FindAsync(id);

            if (employeeAttendance == null)
            {
                return NotFound();
            }

            _context.EmployeeAttendance.Remove(employeeAttendance);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
