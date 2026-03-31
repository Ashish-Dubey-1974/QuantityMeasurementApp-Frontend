using ModelLayer.Interfaces;
using ModelLayer.Enums;

namespace BusinessLayer.Services
{
    public class LengthUnitConverter: IMeasurable<LengthUnit>
    {
        private readonly double[] BaseFactors =
        {
            1.0,
            12.0,
            36.0,
            0.393701,
            0.0393700
        };

        public double GetConversionFactor(LengthUnit unit)
        {
            return BaseFactors[(int)unit];
        }

        public double ConvertToBase(LengthUnit unit, double amount)
        {
            return amount * GetConversionFactor(unit);
        }

        public double ConvertFromBase(LengthUnit unit, double baseValue)
        {
            return baseValue / GetConversionFactor(unit);
        }

        public string GetSymbol(LengthUnit unit)
        {
            switch (unit)
            {
                case LengthUnit.Inches: return "in";
                case LengthUnit.Feet: return "ft";
                case LengthUnit.Yards: return "yd";
                case LengthUnit.Centimeters: return "cm";
                case LengthUnit.MiliMeters: return "mm";
                default: return unit.ToString().ToLower();
            }
        }
    }
}