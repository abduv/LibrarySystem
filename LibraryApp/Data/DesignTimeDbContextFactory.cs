using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using LibraryApp.Data;

namespace LibraryApp.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            // Укажи здесь имя своего файла базы данных
            optionsBuilder.UseSqlite("Data Source=library.db");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}