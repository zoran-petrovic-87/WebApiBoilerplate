using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebApi.Data;

/// <summary>
/// The application database context class.
/// It will use SQLite database.
/// </summary>
/// <seealso cref="WebApi.Data.AppDbContext" />
public class DevAppDbContext : AppDbContext
{
    /// <summary>
    /// The logger factory. Must be static to prevent memory leak.
    /// </summary>
    private static readonly ILoggerFactory DbLoggerFactory = LoggerFactory.Create(builder =>
    {
        builder.AddConsole();
    });

    /// <summary>
    /// Initializes a new instance of the <see cref="DevAppDbContext"/> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    public DevAppDbContext(IConfiguration configuration) : base(configuration)
    {
    }

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseLoggerFactory(DbLoggerFactory)
            .UseSqlite(Configuration.GetConnectionString("WebApiDatabase"));
    }
}