using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApi.Data;

namespace WebApi.Test.Helpers;

/// <summary>
/// The SQLite in-memory database.
/// </summary>
/// <seealso cref="AppDbContext" />
public class SqliteInMemoryAppDbContext : AppDbContext
{
    public SqliteInMemoryAppDbContext(IConfiguration configuration) : base(configuration)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        options.UseSqlite(connection);
    }
}