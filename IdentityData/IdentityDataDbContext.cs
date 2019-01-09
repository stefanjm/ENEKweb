using IdentityData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityData {
    public class IdentityDataDbContext : IdentityDbContext<ApplicationUser> {
        public IdentityDataDbContext(DbContextOptions<IdentityDataDbContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ForNpgsqlUseIdentityColumns();
        }
    }

}
