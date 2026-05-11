using Microsoft.EntityFrameworkCore;
using LibraryApp.Models;

namespace LibraryApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}