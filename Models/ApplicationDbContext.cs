using Microsoft.EntityFrameworkCore;

namespace hki_2025_registration.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Participant> Participants { get; set; }
    }
}
