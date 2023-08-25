using Microsoft.EntityFrameworkCore;

namespace StudentManagement.Models.Entities;

public partial class StudentManagementContext : DbContext
{
    public StudentManagementContext()
    {
    }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionString = "Server=localhost;Database=StudentDb;User Id=sa;Password=A!VeryComplex123Password;MultipleActiveResultSets=true;Trusted_Connection=False;TrustServerCertificate=True";
        optionsBuilder.UseSqlServer(connectionString);
        //base.OnConfiguring(optionsBuilder);
    }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
