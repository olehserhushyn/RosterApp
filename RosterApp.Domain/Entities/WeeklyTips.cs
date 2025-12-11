namespace RosterApp.Domain.Entities
{
    public class WeeklyTips : BaseEntity
    {
        public int WeekNumber { get; private set; }
        public int Year { get; private set; }
        public DateOnly WeekStartDate { get; private set; }
        public decimal TotalAmount { get; private set; }

        public int CurrencyId { get; private set; }
        public Currency Currency { get; private set; }

        private WeeklyTips() { }

        public WeeklyTips(int weekNumber, int year, DateOnly weekStartDate, int currencyId, decimal totalAmount = 0)
        {
            WeekNumber = weekNumber;
            Year = year;
            WeekStartDate = weekStartDate;
            TotalAmount = totalAmount;
            CurrencyId = currencyId;

            Validate();
        }

        public void UpdateAmount(decimal amount, int currencyId)
        {
            TotalAmount = amount;
            CurrencyId = currencyId;

            Validate();

            MarkAsUpdated();
        }

        public void AddAmount(decimal amount)
        {
            TotalAmount += amount;
            MarkAsUpdated();
        }

        public override void Validate()
        {
            if (WeekNumber < 1 || WeekNumber > 53) // can we have 54 weeks?
            { 
                throw new ArgumentException("Week number must be between 1 and 53", nameof(WeekNumber)); 
            }

            if (Year < 2000)
            { 
                throw new ArgumentException("Year must be 2000 or later", nameof(Year)); 
            }

            if (TotalAmount < 0)
            { 
                throw new ArgumentException("Amount cannot be negative", nameof(TotalAmount)); 
            }
        }
    }
}
