using Microsoft.EntityFrameworkCore;
using TestAPI2024.Models;

namespace TestAPI2024.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Student> students { get; set; }    

    }
}
