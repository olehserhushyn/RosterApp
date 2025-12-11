namespace RosterApp.Application.DTOs.WeeklyTips.Requests
{
    public record UpdateWeeklyTipsRequest(
        int Id,
        decimal TotalAmount,
        int? CurrencyId);
}
