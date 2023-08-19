using Microsoft.Extensions.Configuration;

namespace Common;

/// <summary>
/// Database config builder class used to build connection strings for databases.
/// </summary>
public class DatabaseConfigBuilder
{
    private string? DatabaseHost { get; set; }
    private string? DatabasePort { get; set; }
    private string? DatabaseUser { get; set; }
    private string? DatabasePassword { get; set; }
    private string? DatabaseName { get; set; }
    private DatabaseConfigBuilder() { }
    public static DatabaseConfigBuilder New() => new();

    /// <summary>
    /// Method copies database information from the IConfiguration object into the builder.
    /// This method does not populate DatabaseName.
    /// </summary>
    /// <param name="config">IConfiguration object.</param>
    /// <returns>Database config builder</returns>
    public DatabaseConfigBuilder SetFromConfig(IConfiguration config)
    {
        var databaseAddress = config[Constants.DatabaseAddress];
        DatabaseUser = config[Constants.DatabaseUser];
        DatabasePassword = config[Constants.DatabasePassword];

        var split = databaseAddress?.Split(":");
        if (split == null || split.Length <= 1) return this;
        DatabaseHost = split[0];
        DatabasePort = split[1];
        return this;
    }

    /// <summary>
    /// Method for adding database name to the builder.
    /// </summary>
    /// <param name="databaseName">Database name.</param>
    /// <returns>Database config builder</returns>
    public DatabaseConfigBuilder AddName(string databaseName)
    {
        DatabaseName = databaseName;
        return this;
    }
    /// <summary>
    /// Finish the build for the Postgres sql database.
    /// </summary>
    /// <returns>Connection string for Postgres sql database.</returns>
    public string BuildPostgres()
    {
        return $"Server={DatabaseHost};Port={DatabasePort};Database={DatabaseName};UserId={DatabaseUser};Password={DatabasePassword};";
    }
}
