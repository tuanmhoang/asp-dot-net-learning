using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

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
        var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
        var dbName = Environment.GetEnvironmentVariable("DB_NAME");
        var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD");
        var connectionString = string.Empty;
        if(dbHost == null && dbName == null && dbPassword == null) {
            connectionString = "Server=localhost;Database=StudentDb;User Id=sa;Password=Hello@135;MultipleActiveResultSets=true;Trusted_Connection=False;TrustServerCertificate=True";
        } else
        {
            connectionString = $"Server={dbHost};Database={dbName};User ID=sa;Password={dbPassword};MultipleActiveResultSets=true;Trusted_Connection=False;TrustServerCertificate=True";
        }
        //string connectionString = "Server=localhost;Database=StudentDb;User Id=sa;Password=Hello@135;MultipleActiveResultSets=true;Trusted_Connection=False;TrustServerCertificate=True";
        optionsBuilder.UseSqlServer(connectionString);
        //base.OnConfiguring(optionsBuilder);
    }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
