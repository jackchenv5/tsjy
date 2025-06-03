namespace Faoem.Common.Options;

public class EmailOptions
{
    public string Host { get; set; } = null!;
    public int Port { get; set; } = 587;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string From { get; set; } = "FA OEM App";

    // ReSharper disable CollectionNeverUpdated.Global
    public List<string>? BlackList { get; set; }
    public List<string>? WhiteList { get; set; }
}