namespace Tsjy.Dtos
{
    public class RollDiameterDto
    {
        public double RealRollDiameter { get; set; }
        public double ReasonableRollDiameterMin { get; set; }
        public double ReasonableRollDiameterMax { get; set; }
        public long Time { get; set; }
        public double? Value { get; set; }
    }
}