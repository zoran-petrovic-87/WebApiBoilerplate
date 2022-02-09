using WebApi.Data;
using WebApi.Helpers;

namespace WebApi.Test.Helpers;

public class DataHelper
{
    /// <summary>
    /// Gets or sets the <c>Factory</c>, used to generate fixture data.
    /// </summary>
    public Factory Factory { get; set; }

    /// <summary>
    /// Creates the "in memory" database context and seeds it with fixtures.
    /// </summary>
    /// <returns>The database context seeded with fixtures.</returns>
    public AppDbContext CreateDbContext(IPasswordHelper passwordHelper)
    {
        // Create instance of DbContext.
        var dbContext = new SqliteInMemoryAppDbContext(null);
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        Factory = new Factory(dbContext, passwordHelper);

        return dbContext;
    }
}