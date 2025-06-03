namespace Faoem.Variable.Definitions;

public class AppConnectionStatus(string name, string status)
{
    public string Name { get; set; } = name;
    public string Status { get; set; } = status;
}