using Microsoft.EntityFrameworkCore;

namespace hki_2025_registration.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Participant> Participants { get; set; }
        public DbSet<ShopKeeper> ShopKeepers { get; set; } = default!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Participant>()
                .HasIndex(p => p.PaymentId)
                .IsUnique();

            modelBuilder.Entity<Participant>()
                .HasIndex(p => p.Contact);
        }
    }
}
