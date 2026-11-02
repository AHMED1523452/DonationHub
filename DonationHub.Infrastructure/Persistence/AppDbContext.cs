using DonationHub.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DonationHub.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<ApplicationUser,IdentityRole<Guid>, Guid>
    {
        private readonly IConfiguration _configuration;

        public AppDbContext(DbContextOptions<AppDbContext> options,
                            IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<Category> Categories{ get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<IdempotencyRecord> IdempotencyRecords { get; set; }
        public DbSet<PaymentAttempt> PaymentAttempts { get; set; }
        public DbSet<ProcessedStripeEvent> ProcessedStripeEvents { get; set; }
        public DbSet<RefreshTokens> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            modelBuilder.Entity<IdentityRole<Guid>>()
                        .HasData(GetSeedRoles());
        }
         
        //. not best practice 
        private IEnumerable<IdentityRole<Guid>> GetSeedRoles()
        {
            return _configuration.GetSection("Identity:Roles").GetChildren().Select(role =>
            {
                var id = role["Id"] ?? throw new InvalidOperationException("Each seeded role requires an Id.");
                var name = role["Name"] ?? throw new InvalidOperationException("Each seeded role requires a Name.");

                return new IdentityRole<Guid>
                {
                    Id = Guid.Parse(id),
                    Name = name,
                    NormalizedName = name.ToUpperInvariant(),
                    ConcurrencyStamp = $"seed-role-{name.ToUpperInvariant()}"
                };
            }).ToArray();
        }
    }
}
