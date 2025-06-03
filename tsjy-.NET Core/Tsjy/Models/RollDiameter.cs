namespace Tsjy.Models
{
    public class RollDiameter
    {
        public int Id { get; set; }
        public required string MotorId { get; set; }
        public double RealRollDiameter { get; set; }
        public double ReasonableRollDiameterMin { get; set; }
        public double ReasonableRollDiameterMax { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}