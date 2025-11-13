using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cldv6212_RetailAppPartThree.Models;

namespace Cldv6212_RetailAppPartThree.Data
{
    public class Cldv6212_RetailAppPartThreeContext : DbContext
    {
        public Cldv6212_RetailAppPartThreeContext (DbContextOptions<Cldv6212_RetailAppPartThreeContext> options)
            : base(options)
        {
        }

        public DbSet<Cldv6212_RetailAppPartThree.Models.User> User { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
        }
    }
}
