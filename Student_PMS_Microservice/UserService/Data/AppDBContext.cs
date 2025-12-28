using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> User { get; set; }
    public DbSet<Role> Role { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigureUser(modelBuilder);
        ConfigureRole(modelBuilder);
    }
    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            // User → Role (RoleID)
            entity.HasOne(u => u.Role)
                  .WithMany() // or WithMany(r => r.Users) if you add ICollection<User> Users in Role
                  .HasForeignKey(u => u.RoleID)
                  .OnDelete(DeleteBehavior.Restrict);

            // User → CreatedBy (User)
            entity.HasOne(u => u.CreatedBy)
                  .WithMany()
                  .HasForeignKey(u => u.CreatedByID)
                  .OnDelete(DeleteBehavior.NoAction);

            // User → ModifiedBy (User)
            entity.HasOne(u => u.ModifiedBy)
                  .WithMany()
                  .HasForeignKey(u => u.ModifiedByID)
                  .OnDelete(DeleteBehavior.NoAction);
        });
    }
    private static void ConfigureRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            // Role → CreatedBy (User)
            entity.HasOne(r => r.CreatedBy)
                  .WithMany()
                  .HasForeignKey(r => r.CreatedByID)
                  .OnDelete(DeleteBehavior.NoAction);

            // Role → ModifiedBy (User)
            entity.HasOne(r => r.ModifiedBy)
                  .WithMany()
                  .HasForeignKey(r => r.ModifiedByID)
                  .OnDelete(DeleteBehavior.NoAction);
        });
    }
}