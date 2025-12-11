using Microsoft.AspNetCore.Mvc;
using RosterApp.API.DTOs.Employees.Requests;
using RosterApp.API.DTOs.Employees.Responses;
using RosterApp.Application.Abstractions.Services;

namespace RosterApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDto>> GetById(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound($"Employee with ID {id} not found");
            }

            return Ok(employee);
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeDto>> Create([FromBody] CreateEmployeeRequest request)
        {
            var employee = await _employeeService.CreateEmployeeAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EmployeeDto>> Update(int id, [FromBody] UpdateEmployeeRequest request)
        {
            var employee = await _employeeService.UpdateEmployeeAsync(id, request);
            return Ok(employee);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _employeeService.DeleteEmployeeAsync(id);
            return NoContent();
        }
    }
}
