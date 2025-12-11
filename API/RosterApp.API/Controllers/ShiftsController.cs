using Microsoft.AspNetCore.Mvc;
using RosterApp.Application.Abstractions.Services;
using RosterApp.Application.DTOs.Shifts.Requests;
using RosterApp.Application.DTOs.Shifts.Responses;

namespace RosterApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShiftsController : ControllerBase
    {
        private readonly IShiftService _shiftService;

        public ShiftsController(IShiftService shiftService)
        {
            _shiftService = shiftService;
        }

        [HttpGet("week/{year}/{weekNumber}")]
        public async Task<ActionResult<IEnumerable<ShiftDto>>> GetByWeek(int year, int weekNumber)
        {
            var shifts = await _shiftService.GetShiftsByWeekAsync(weekNumber, year);
            return Ok(shifts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ShiftDto>> GetById(int id)
        {
            var shift = await _shiftService.GetShiftByIdAsync(id);
            if (shift == null)
            {
                return NotFound($"Shift with ID {id} not found");
            }

            return Ok(shift);
        }

        [HttpPost]
        public async Task<ActionResult<ShiftDto>> Create([FromBody] CreateShiftRequest request)
        {
            var shift = await _shiftService.CreateShiftAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = shift.Id }, shift);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ShiftDto>> Update(int id, [FromBody] UpdateShiftRequest request)
        {
            var shift = await _shiftService.UpdateShiftAsync(id, request);
            return Ok(shift);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _shiftService.DeleteShiftAsync(id);
            return NoContent();
        }
    }
}
