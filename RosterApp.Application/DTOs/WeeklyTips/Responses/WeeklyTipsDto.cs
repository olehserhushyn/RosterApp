namespace RosterApp.Application.DTOs.WeeklyTips.Responses
{
    public record WeeklyTipsDto(
        int Id,
        int WeekNumber,
        int Year,
        DateOnly WeekStartDate,
        decimal TotalAmount,
        string CurrencyCode,
        string CurrencySymbol);
}
