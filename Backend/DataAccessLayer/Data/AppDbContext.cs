using Microsoft.EntityFrameworkCore;
using ModelLayer.Models;

namespace DataAccessLayer.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options){}
        
        public DbSet<QuantityMeasurement> QuantityMeasurement {get; set;}
        public DbSet<User> Users { get; set; } 
    }
}