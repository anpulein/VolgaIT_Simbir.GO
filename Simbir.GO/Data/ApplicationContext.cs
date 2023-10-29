using Microsoft.EntityFrameworkCore;
using Simbir.GO.Models;
using Simbir.GO.Models.Transport;

namespace Simbir.GO.Data;

public sealed class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
    : base(options)
    {
        Database.Migrate();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>().HasData(
            new Account
            {
                Id = 1,
                Username = "admin", 
                Password = BCrypt.Net.BCrypt.HashPassword("admin"),
                IsAdmin = true
            }
        );
    }

    /// <summary>
    /// Account model
    /// </summary>
    public DbSet<Account> Accounts { get; set; } = null!;
    /// <summary>
    /// TransportInfo model
    /// </summary>
    public DbSet<TransportInfo> Transports { get; set; } = null!;
    
}
