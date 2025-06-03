using System.ComponentModel.DataAnnotations;

namespace Tsjy.Dtos;

public class GetCraftDataDto
{
    [Required] public long StartTime { get; set; }
    [Required] public long EndTime { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public long FacilityId { get; set; }
}