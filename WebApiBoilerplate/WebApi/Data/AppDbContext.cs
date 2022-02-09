using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApi.Models;

namespace WebApi.Data;

/// <summary>
/// The application database context class.
/// It will use Postgres database.
/// </summary>
/// <seealso cref="Microsoft.EntityFrameworkCore.DbContext" />
public class AppDbContext : DbContext
{
    /// <summary>
    /// The application configuration.
    /// </summary>
    protected readonly IConfiguration Configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext"/> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    public AppDbContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(Configuration.GetConnectionString("WebApiDatabase"));
    }

    /// <summary>
    /// Gets or sets the users.
    /// </summary>
    /// <value>
    /// The users.
    /// </value>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Gets or sets the roles.
    /// </summary>
    /// <value>
    /// The roles.
    /// </value>
    public DbSet<Role> Roles { get; set; }

    /// <summary>
    /// Gets or sets the logs.
    /// </summary>
    /// <value>
    /// The logs.
    /// </value>
    public DbSet<Log> Logs { get; set; }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(b =>
        {
            // Property.
            b.Property(x => x.Id).ValueGeneratedNever();
            b.Property(x => x.Username).IsRequired().HasMaxLength(255);
            b.Property(x => x.GivenName).HasMaxLength(30);
            b.Property(x => x.FamilyName).HasMaxLength(30);
            b.Property(x => x.Email).HasMaxLength(320);
            b.Property(x => x.CreatedAt).IsRequired();

            // Index.
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.Username);
            b.HasIndex(x => x.IsActive);
            b.HasIndex(x => x.RoleId);
            b.HasIndex(x => x.CreatedById);
            b.HasIndex(x => x.UpdatedById);

            // Relation.
            b.HasOne(x => x.Role).WithMany(y => y.Users).HasForeignKey(z => z.RoleId);
            b.HasOne(x => x.CreatedBy).WithMany(x => x.CreatedUsers).HasForeignKey(x => x.CreatedById);
            b.HasOne(x => x.UpdatedBy).WithMany(x => x.UpdatedUsers).HasForeignKey(x => x.UpdatedById);
        });

        modelBuilder.Entity<Role>(b =>
        {
            // Property.
            b.Property(x => x.Id).ValueGeneratedNever();
            b.Property(x => x.Name).IsRequired().HasMaxLength(64);

            // Index.
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.Name).IsUnique();
            b.HasIndex(x => x.CreatedById);
            b.HasIndex(x => x.UpdatedById);

            // Relation.
            b.HasOne(x => x.CreatedBy).WithMany(x => x.CreatedRoles).HasForeignKey(x => x.CreatedById);
            b.HasOne(x => x.UpdatedBy).WithMany(x => x.UpdatedRoles).HasForeignKey(x => x.UpdatedById);
        });
    }
}