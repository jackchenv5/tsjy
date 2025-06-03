using Tsjy.Models;

namespace Tsjy.Dtos;

public class AlarmCountDto
{
    public string Message { get; set; } = null!;
    public int Count { get; set; }
}