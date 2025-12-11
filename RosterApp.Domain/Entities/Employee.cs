namespace RosterApp.Domain.Entities
{
    public class Employee : BaseEntity
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }


        private readonly List<Shift> _shifts = new();
        public IReadOnlyCollection<Shift> Shifts => _shifts.AsReadOnly();

        private Employee() { }

        public Employee(string firstName, string lastName, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;

            Validate();
        }

        public void UpdateDetails(string firstName, string lastName, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;

            Validate();

            MarkAsUpdated();
        }

        public void AddShift(Shift shift)
        {
            if (shift == null)
            {
                throw new ArgumentNullException(nameof(shift));
            }
                
            _shifts.Add(shift);
            MarkAsUpdated();
        }

        public void RemoveShift(Shift shift)
        {
            if (shift == null)
                throw new ArgumentNullException(nameof(shift));

            _shifts.Remove(shift);
            MarkAsUpdated();
        }

        public override void Validate()
        {
            if (string.IsNullOrWhiteSpace(FirstName))
            {
                throw new ArgumentException("First name cannot be empty", nameof(FirstName));
            }
                
            if (string.IsNullOrWhiteSpace(LastName))
            {
                throw new ArgumentException("Last name cannot be empty", nameof(LastName));
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                throw new ArgumentException("Email cannot be empty", nameof(Email));
            }    
        }
    }
}
