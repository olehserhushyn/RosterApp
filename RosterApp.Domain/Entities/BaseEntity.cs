namespace RosterApp.Domain.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }
        public bool IsDeleted { get; protected set; }

        protected BaseEntity()
        {
            var now = DateTime.UtcNow;
            CreatedAt = now;
            UpdatedAt = now;
            IsDeleted = false;
        }

        public void MarkAsUpdated()
        {
            UpdatedAt = DateTime.UtcNow;
        }

        public void Delete()
        {
            IsDeleted = true;
            MarkAsUpdated();
        }
        public abstract void Validate();
    }
}
