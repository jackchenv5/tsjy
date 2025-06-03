namespace Faoem.Variable.Options;

public class InfluxDbOptions
{
    public string Url { get; set; } = null!;
    public string Org { get; set; } = "faoem";
    public string Bucket { get; set; } = "faoem_default_bucket";
    public string Measurement { get; set; } = null!;
    public string Token { get; set; } = null!;
}