using System.ComponentModel.DataAnnotations;

namespace RosterApp.Domain.Entities
{
    public class Shift : BaseEntity
    {
        public DateOnly Date { get; private set; }
        public TimeOnly StartTime { get; private set; }
        public TimeOnly EndTime { get; private set; }
        [MaxLength(500)]
        public string? Notes { get; private set; }

        public int EmployeeId { get; private set; }
        public Employee Employee { get; private set; }

        private Shift() { }

        public Shift(int employeeId, DateOnly date, TimeOnly startTime, TimeOnly endTime, string? notes = null)
        {
            EmployeeId = employeeId;
            Date = date;
            StartTime = startTime;
            EndTime = endTime;
            Notes = notes;

            Validate();
        }

        public void Update(DateOnly date, TimeOnly startTime, TimeOnly endTime, string notes = null)
        {
            Date = date;
            StartTime = startTime;
            EndTime = endTime;
            Notes = notes;

            Validate();

            MarkAsUpdated();
        }

        public override void Validate()
        {
            if (EmployeeId <= 0)
            {
                throw new ArgumentException("Invalid Employee ID", nameof(EmployeeId));
            }

            if (Date == default)
            {
                throw new ArgumentException("Shift date cannot be empty", nameof(Date));
            }

            if (StartTime == default && EndTime == default)
            {
                throw new ArgumentException("StartTime and EndTime must be set");
            }

            if (StartTime >= EndTime)
            {
                throw new ArgumentException("Shift StartTime must be earlier than EndTime");
            }

            var duration = EndTime.ToTimeSpan() - StartTime.ToTimeSpan();

            if (duration.TotalMinutes <= 0)
            {
                throw new ArgumentException("Shift duration must be greater than zero minutes");
            }

            if (duration.TotalHours > 24)
            {
                throw new ArgumentException("Shift duration cannot exceed 24 hours");
            }
        }
    }
}
