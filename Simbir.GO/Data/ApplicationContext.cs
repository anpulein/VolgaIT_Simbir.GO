using Microsoft.EntityFrameworkCore;
using Simbir.GO.Models;
using Simbir.GO.Models.Rent;
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
        modelBuilder.Entity<AccountInfo>().HasData(
            new AccountInfo
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
    public DbSet<AccountInfo> Accounts { get; set; } = null!;
    /// <summary>
    /// Transport model
    /// </summary>
    public DbSet<TransportInfo> Transports { get; set; } = null!;
    /// <summary>
    /// Rent model
    /// </summary>
    public DbSet<Rental> Rents { get; set; } = null!;

}
