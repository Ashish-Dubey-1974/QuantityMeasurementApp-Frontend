using ModelLayer.Enums;

namespace ModelLayer.Interfaces
{
    /// <summary>
    /// Defines the basic operations required for measurable units.
    /// Any unit type implementing this should support conversion
    /// to and from a base unit and provide a display symbol.
    /// </summary>
    public interface IMeasurable<TUnit>: IUnitConverter<TUnit> where TUnit: struct, Enum
    {
        double GetConversionFactor(TUnit unit);
    }
}