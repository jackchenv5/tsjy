using System.ComponentModel.DataAnnotations;

namespace Tsjy.Dtos;

public class GetProductionStatisticsDto
{
    [Required] public long FacilityId { get; set; }
    [Required] public long StartTime { get; set; }
    [Required] public long EndTime { get; set; }
}