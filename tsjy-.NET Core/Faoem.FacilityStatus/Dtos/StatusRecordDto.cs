namespace Faoem.FacilityStatus.Dtos;

public class StatusRecordDto
{
    public long Time { get; set; }
    public string Status { get; set; }
    public Shift.Models.Shift Shift { get; set; }
    public int Duration { get; set; }

    public StatusRecordDto(long time, string status, Shift.Models.Shift shift, int duration)
    {
        Time = time;
        Status = status;
        Shift = shift;
        Duration = duration;
    }
}