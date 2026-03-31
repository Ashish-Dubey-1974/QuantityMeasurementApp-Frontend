namespace ModelLayer.DTOs
{
    public class AddRequestDto
    {
        public required string QuantityType {get; set;}
        public double Value1 {get; set;}
        public double Value2 {get; set;}
        public required string Unit1 {get; set;}
        public required string Unit2 {get; set;}
    }
}