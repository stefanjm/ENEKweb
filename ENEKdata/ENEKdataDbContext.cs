using ENEKdata.Models;
using ENEKdata.Models.Leiunurk;
using Microsoft.EntityFrameworkCore;
using System;

namespace ENEKdata {
    public class ENEKdataDbContext : DbContext {
        public ENEKdataDbContext(DbContextOptions<ENEKdataDbContext> options) : base(options) { }

        public DbSet<Item> Items { get; set; }
        public DbSet<Image> Images { get; set; }
    }
}
