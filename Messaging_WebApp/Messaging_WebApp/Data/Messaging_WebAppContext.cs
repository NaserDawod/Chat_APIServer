using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Messaging_WebApp.Models;

/*
namespace Messaging_WebApp.Data
{
    public class Messaging_WebAppContext : DbContext
    {
        private const string connectionString = "server=localhost;port=3306;database=myUsers;user=root;password=ansa@12";
        
        public Messaging_WebAppContext(DbContextOptions<Messaging_WebAppContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(connectionString, MariaDbServerVersion.AutoDetect(connectionString));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuring the Name property as the primary
            // key of the Items table
            modelBuilder.Entity<User>().HasKey(e => e.Username);
            modelBuilder.Entity<Contact>().HasKey(e => e.Id);
            modelBuilder.Entity<Message>().HasKey(e => e.Id);
        }

        public DbSet<User>? User { get; set; }
        public DbSet<Contact>? Contact { get; set; }
        public DbSet<Message>? Message { get; set; }
    }
}
*/
namespace Messaging_WebApp.Data
{
    public class Messaging_WebAppContext : DbContext
    {
        public Messaging_WebAppContext (DbContextOptions<Messaging_WebAppContext> options)
            : base(options)
        {
        }

        public DbSet<Messaging_WebApp.Models.User>? User { get; set; }

        public DbSet<Messaging_WebApp.Models.Contact>? Contact { get; set; }

        public DbSet<Messaging_WebApp.Models.Message>? Message { get; set; }
    }
}
