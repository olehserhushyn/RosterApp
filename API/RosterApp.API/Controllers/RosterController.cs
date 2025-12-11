using Microsoft.AspNetCore.Mvc;
using RosterApp.Application.Abstractions.Services;
using RosterApp.Application.Common.Utils;
using RosterApp.Application.DTOs.WeeklyTips.Responses;

namespace RosterApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RosterController : ControllerBase
    {
        private readonly IShiftService _shiftService;

        public RosterController(IShiftService shiftService)
        {
            _shiftService = shiftService;
        }

        [HttpGet("week/{year}/{weekNumber}")]
        public async Task<ActionResult<WeeklyRosterDto>> GetWeeklyRoster(int year, int weekNumber)
        {
                var roster = await _shiftService.GetWeeklyRosterAsync(weekNumber, year);
                return Ok(roster);
        }

        [HttpGet("current")]
        public async Task<ActionResult<WeeklyRosterDto>> GetCurrentWeekRoster()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var (year, weekNumber) = WeekUtils.GetWeekNumber(today);

            var roster = await _shiftService.GetWeeklyRosterAsync(weekNumber, year);
            return Ok(roster);
        }
    }
}
