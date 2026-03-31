namespace ModelLayer.Models
{
    public class QuantityMeasurement
    {
        public int Id { get; set; }

        public string Category { get; set; }

        public string Operation { get; set; }

        public double Value1 { get; set; }

        public string Unit1 { get; set; }

        public double? Value2 { get; set; }   // nullable

        public string? Unit2 { get; set; }    // nullable

        public double ResultValue { get; set; }

        public string ResultUnit { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}