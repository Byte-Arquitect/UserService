using Microsoft.EntityFrameworkCore;
using User_Service.Src.Models;

public class UserServiceContext : DbContext
{
    public UserServiceContext(DbContextOptions<UserServiceContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserProgress> UserProgresses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        
    }
}