using System.ComponentModel.DataAnnotations;

namespace Tsjy.Dtos;

public class GetAlarmHistoryDto
{
    [Required] public long StartTime { get; set; }
    [Required] public long EndTime { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}