namespace Tsjy.Dtos;

public class CraftHistoryDto
{
    public int Index { get; set; }

    public string MaterialSpecification { get; set; } = string.Empty;
    public double SectionHeight { get; set; }
    public double InfeedVelocity { get; set; }
    public double NewLineSpeed { get; set; }
    public double LineVelocity { get; set; }
}