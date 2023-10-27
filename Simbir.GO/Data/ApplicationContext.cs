using Microsoft.EntityFrameworkCore;
using Simbir.GO.Models;

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
}
