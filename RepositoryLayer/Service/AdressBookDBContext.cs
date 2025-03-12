using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Model;

namespace RepositoryLayer.Service
{
    public class AdressBookDBContext : DbContext
    {
        public AdressBookDBContext(DbContextOptions<AdressBookDBContext> options) : base(options) { }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<AddressBookEntryEntity> AddressBookEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
