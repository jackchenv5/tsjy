using System.ComponentModel.DataAnnotations;

namespace Tsjy.Dtos;

public class GetPartMaintainHistoryDto
{
    [Required] public long StartTime { get; set; }
    [Required] public long EndTime { get; set; }
    public long FacilityId { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}