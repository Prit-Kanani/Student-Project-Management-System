using Microsoft.EntityFrameworkCore;
using ProjectGroupServices.Models;
using System.Collections.Generic;

namespace ProjectGroupServices.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<ProjectGroup> ProjectGroup { get; set; }
    public DbSet<GroupWiseStudent> GroupWiseStudent { get; set; }
    public DbSet<ProjectGroupByProject> ProjectGroupByProject { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GroupWiseStudent>()
                .HasIndex(gs => new { gs.ProjectGroupID, gs.StudentID })
                .IsUnique();

        base.OnModelCreating(modelBuilder);
    }

}