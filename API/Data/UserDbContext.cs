using API.Entity;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UserDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    
    private string DbPath { get; }

    public UserDbContext()
    {
        const Environment.SpecialFolder folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "user.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DbPath}");
    }
}