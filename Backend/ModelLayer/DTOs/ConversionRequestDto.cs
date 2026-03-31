namespace ModelLayer.DTOs
{
    public class ConversionRequestDto
    {
        public required string QuantityType {get; set;}
        public double Value {get; set;}
        public required string SourceUnit {get; set;}
        public required string TargetUnit {get; set;}
    }
}