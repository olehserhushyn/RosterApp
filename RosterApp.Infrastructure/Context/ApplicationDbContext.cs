using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RosterApp.Domain.Entities;

namespace RosterApp.Infrastructure.Context
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Shift> Shifts { get; set; } = null!;
        public DbSet<Currency> Currencies { get; set; } = null!;
        public DbSet<WeeklyTips> WeeklyTips { get; set; } = null!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureEmployee(modelBuilder);
            ConfigureShift(modelBuilder);
            ConfigureCurrency(modelBuilder);
            ConfigureWeeklyTips(modelBuilder);
        }

        private static void ConfigureEmployee(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Employee>();

            entity.ToTable("Employees");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(200);

            entity.HasMany(e => e.Shifts)
                .WithOne(s => s.Employee)
                .HasForeignKey(s => s.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(e => !e.IsDeleted);
        }

        private static void ConfigureShift(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Shift>();

            entity.ToTable("Shifts");

            entity.HasKey(s => s.Id);

            entity.Property(s => s.Date)
                .IsRequired();

            entity.Property(s => s.StartTime)
                .IsRequired();

            entity.Property(s => s.EndTime)
                .IsRequired();

            entity.Property(s => s.Notes)
                .HasMaxLength(500);

            entity.Property(s => s.EmployeeId)
                .IsRequired();

            entity.HasIndex(s => s.Date);
            entity.HasIndex(s => new { s.EmployeeId, s.Date });

            entity.HasQueryFilter(s => !s.IsDeleted);
        }

        private static void ConfigureCurrency(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Currency>();

            entity.ToTable("Currencies");

            entity.HasKey(c => c.Id);

            entity.Property(c => c.Code)
                .IsRequired()
                .HasMaxLength(3);

            entity.Property(c => c.Symbol)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(c => c.Code)
                .IsUnique();

            entity.HasQueryFilter(c => !c.IsDeleted);
        }

        private static void ConfigureWeeklyTips(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<WeeklyTips>();

            entity.ToTable("WeeklyTips");

            entity.HasKey(w => w.Id);

            entity.Property(w => w.WeekNumber)
                .IsRequired();

            entity.Property(w => w.Year)
                .IsRequired();

            entity.Property(w => w.WeekStartDate)
                .IsRequired();

            entity.Property(w => w.TotalAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(w => w.CurrencyId)
                .IsRequired();

            entity.HasOne(w => w.Currency)
                .WithMany()
                .HasForeignKey(w => w.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(w => new { w.Year, w.WeekNumber })
                .IsUnique();

            entity.HasQueryFilter(w => !w.IsDeleted);
        }

        public override int SaveChanges()
        {
            ValidateEntities();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ValidateEntities();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void ValidateEntities()
        {
            var entries = ChangeTracker
                .Entries<BaseEntity>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                entry.Entity.Validate();
            }
        }
    }
}
