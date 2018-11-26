using ENEKdata.Models.Leiunurk;
using ENEKdata.Models.Partnerid;
using ENEKdata.Models.TehtudTood;
using Microsoft.EntityFrameworkCore;

namespace ENEKdata {
    public class ENEKdataDbContext : DbContext {
        public ENEKdataDbContext(DbContextOptions<ENEKdataDbContext> options) : base(options) { }

        // Leiunurk
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemImage> ItemImages { get; set; }

        // Partnerid
        public DbSet<Partner> Partners { get; set; }
        public DbSet<PartnerImage> PartnerImages { get; set; }

        // Tehtud tööd
        public DbSet<TehtudToo> TehtudTood { get; set; }
        public DbSet<TehtudTooImage> TehtudTooImages { get; set; }

    }
}
