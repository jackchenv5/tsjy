namespace Tsjy.Dtos;

public class CraftDataDto
{
    public int Id { get; set; }
    public long Time { get; set; }
    public string CustomerCode { get; set; } = string.Empty;
    public string MaterialSpecification { get; set; } = string.Empty;
    public List<CraftHistoryDto> Children { get; set; } = [];
}