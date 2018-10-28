using ENEKdata.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace ENEKdata {
    public class ENEKdataDbContext : DbContext {
        public ENEKdataDbContext(DbContextOptions<ENEKdataDbContext> options) : base(options) { }

        public DbSet<Item> Items { get; set; }
    }
}
