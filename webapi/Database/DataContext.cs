using Microsoft.EntityFrameworkCore;
using webapi.Models;

namespace webapi.Database
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }

        public DbSet<Movie> Movies => Set<Movie>();
        public DbSet<Rating> Ratings => Set<Rating>();
    }
}
