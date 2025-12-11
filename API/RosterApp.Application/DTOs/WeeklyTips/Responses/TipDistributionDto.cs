namespace RosterApp.Application.DTOs.WeeklyTips.Responses
{
    public record TipDistributionDto(
       int WeekNumber,
       int Year,
       DateOnly WeekStartDate,
       decimal TotalTips,
       string CurrencyCode,
       string CurrencySymbol,
       double TotalHours,
       List<EmployeeTipShareDto> EmployeeShares);
}
