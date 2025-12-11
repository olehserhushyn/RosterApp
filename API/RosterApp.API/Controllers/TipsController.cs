using Microsoft.AspNetCore.Mvc;
using RosterApp.Application.Abstractions.Services;
using RosterApp.Application.Common.Utils;
using RosterApp.Application.DTOs.WeeklyTips.Responses;

namespace RosterApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TipsController : ControllerBase
    {
        private readonly ITipDistributionService _tipDistributionService;
        private readonly ITipsService _tipsService;

        public TipsController(ITipDistributionService tipDistributionService, ITipsService tipsService)
        {
            _tipDistributionService = tipDistributionService;
            _tipsService = tipsService;
        }

        [HttpGet("distribution/week/{year}/{weekNumber}")]
        public async Task<ActionResult<TipDistributionDto>> GetWeeklyDistribution(int year, int weekNumber)
        {
            var distribution = await _tipDistributionService.GetWeeklyDistributionAsync(
                weekNumber, year);
            return Ok(distribution);

        }

        [HttpGet("distribution/current")]
        public async Task<ActionResult<TipDistributionDto>> GetCurrentWeekDistribution()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var (year, weekNumber) = WeekUtils.GetWeekNumber(today);

            var distribution = await _tipDistributionService.GetWeeklyDistributionAsync(
                weekNumber, year);
            return Ok(distribution);
        }

        [HttpGet("week/{year}/{weekNumber}")]
        public async Task<ActionResult<WeeklyTipsDto>> GetWeeklyTips(int year, int weekNumber)
        {
            var weeklyTips = await _tipsService.GetWeeklyTips(year, weekNumber);
            return Ok(weeklyTips);
        }
    }
}
