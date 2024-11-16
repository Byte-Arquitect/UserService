using Microsoft.EntityFrameworkCore;
using User_Service.Src.Models;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserProgress> UserProgresses { get; set; } = null!;

    
}