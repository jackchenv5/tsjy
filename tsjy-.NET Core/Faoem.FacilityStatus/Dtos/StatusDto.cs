using Faoem.FacilityStatus.Models;

namespace Faoem.FacilityStatus.Dtos;

public enum Status
{
    Invalid = 0,
    Running = 1,
    Standby = 2,
    Stopped = 3,
    Error = 4
}

public class StatusDto
{
    public int RunningSeconds { get; set; }
    public int StandbySeconds { get; set; }
    public int StoppedSeconds { get; set; }
    public int ErrorSeconds { get; set; }
    public Status CurrentStatus { get; set; }
}