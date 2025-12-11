namespace RosterApp.Application.Common.Utils
{
    public static class WeekUtils
    {
        public static DateOnly GetWeekStartDate(int weekNumber, int year)
        {
            var jan1 = new DateOnly(year, 1, 1);
            var daysOffset = DayOfWeek.Monday - jan1.DayOfWeek;
            var firstMonday = jan1.AddDays(daysOffset);
            return firstMonday.AddDays((weekNumber - 1) * 7);
        }

        public static (int year, int weekNumber) GetWeekNumber(DateOnly date)
        {
            var calendar = System.Globalization.CultureInfo.CurrentCulture.Calendar;
            var weekNumber = calendar.GetWeekOfYear(
                date.ToDateTime(TimeOnly.MinValue),
                System.Globalization.CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Monday);

            return (date.Year, weekNumber);
        }
    }
}
