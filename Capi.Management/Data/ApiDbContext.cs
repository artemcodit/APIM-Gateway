using Capi.Management.Models;
using Microsoft.EntityFrameworkCore;

namespace Capi.Management.Data
{
 /// The database context for the API Management service.
    public class ApiDbContext : DbContext
    {
         /// Initializes a new instance of the <see cref="ApiDbContext"/> class.
        /// <param name="options">The options for this context.</param>
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }

         /// Gets or sets the DbSet for APIs.
        public DbSet<Api> Apis { get; set; }

        public DbSet<ApiProduct> ApiProducts { get; set; }

         /// Gets or sets the DbSet for Policies.
        public DbSet<Policy> Policies { get; set; }

         /// Configures the model for the context.
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the one-to-many relationship between Api and Policy
            modelBuilder.Entity<Api>()
                .HasMany(a => a.Policies)
                .WithOne(p => p.Api)
                .HasForeignKey(p => p.ApiId);

            modelBuilder.Entity<Api>()
                .HasMany(a => a.ApiProducts)
                .WithMany(p => p.Apis)
                .UsingEntity(j => j.ToTable("ApiApiProduct"));
        }
    }
}
