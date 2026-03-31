using ModelLayer.Interfaces;
using ModelLayer.Enums;

namespace BusinessLayer.Services
{
    /// <summary>
    /// Helper methods for weight unit conversions.
    /// Base unit used internally is kilogram.
    /// </summary>
    public class WeightUnitConverter: IMeasurable<WeightUnit>
    {
        private static readonly double[] BaseFactors =
        {
            0.001,
            1.0,
            0.453592
        };

        public double GetConversionFactor(WeightUnit unit)
        {
            return BaseFactors[(int)unit];
        }

        public double ConvertToBase(WeightUnit unit, double amount)
        {
            return amount * GetConversionFactor(unit);
        }

        public double ConvertFromBase(WeightUnit unit, double baseValue)
        {
            return baseValue / GetConversionFactor(unit);
        }

        public string GetSymbol(WeightUnit unit)
        {
            switch (unit)
            {
                case WeightUnit.Grams: return "g";
                case WeightUnit.Kilograms: return "Kg";
                case WeightUnit.Pound: return "lb";
                default: return unit.ToString().ToLower();
            }
        }
    }
}