namespace Tsjy.Dtos;

public class ProductionStatisticsDto
{
    public string CustomerCode { get; set; } = string.Empty;
    public int TotalCount { get; set; }
    public List<MaterialSpecificationDto> MaterialSpecifications { get; set; } = [];
}

public class MaterialSpecificationDto
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
}