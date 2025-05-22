using ContactSystem.Dal.Entities;
using ContactSystem.Dal.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace ContactSystem.Dal;

public class MainContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Role> UserRoles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public MainContext(DbContextOptions<MainContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new ContactConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}