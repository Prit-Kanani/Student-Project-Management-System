using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace ProjectGroup.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> User { get; set; }
    public DbSet<Role> Role { get; set; }
    public DbSet<UserProfile> UserProfile { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigureUser(modelBuilder);
        ConfigureRole(modelBuilder);
        ConfigureUserProfile(modelBuilder);
    }
    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            #region RoleID
            // User → Role (RoleID)
            entity.HasOne(u => u.Role)
                  .WithMany() // or WithMany(r => r.Users) if you add ICollection<User> Users in Role
                  .HasForeignKey(u => u.RoleID)
                  .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region CreatedByID
            // User → CreatedBy (User)
            entity.HasOne(u => u.CreatedBy)
                  .WithMany()
                  .HasForeignKey(u => u.CreatedByID)
                  .OnDelete(DeleteBehavior.NoAction);
            #endregion

            #region ModifiedByID
            // User → ModifiedBy (User)
            entity.HasOne(u => u.ModifiedBy)
                  .WithMany()
                  .HasForeignKey(u => u.ModifiedByID)
                  .OnDelete(DeleteBehavior.NoAction);
            #endregion
        });
    }
    private static void ConfigureRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            #region CreatedByID
            // Role → CreatedBy (User)
            entity.HasOne(r => r.CreatedBy)
                  .WithMany()
                  .HasForeignKey(r => r.CreatedByID)
                  .OnDelete(DeleteBehavior.NoAction);
            #endregion

            #region ModifiedByID
            // Role → ModifiedBy (User)
            entity.HasOne(r => r.ModifiedBy)
                  .WithMany()
                  .HasForeignKey(r => r.ModifiedByID)
                  .OnDelete(DeleteBehavior.NoAction);
            #endregion
        });
    }
    private static void ConfigureUserProfile(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasIndex(up => up.UserID).IsUnique();
        });
    }
}