namespace Faoem.Common.Options;

public class DbOptions
{
    public string Type { get; set; } = "Sqlite";

    public string SqliteConnectionString { get; set; } = "Data Source=Data/Faoem.db";

    public string MySqlConnectionString { get; set; } = "";
}