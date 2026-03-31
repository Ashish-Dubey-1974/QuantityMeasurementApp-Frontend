using ModelLayer.Interfaces;
using ModelLayer.Enums;
namespace BusinessLayer.Services
{

    public class TemperatureUnitConverter: IUnitConverter<TemperatureUnit>
{
    public double ConvertToBase(TemperatureUnit unitType, double inputTemp)
    {
        switch (unitType)
        {
            case TemperatureUnit.Celsius:
                return inputTemp;

            case TemperatureUnit.Fahrenheit:
                return (inputTemp - 32.0) * 5.0 / 9.0;

            case TemperatureUnit.Kelvin:
                return inputTemp - 273.15;

            default:
                throw new ArgumentException("Unsupported temperature unit");
        }
    }

    public double ConvertFromBase(TemperatureUnit unitType, double celsiusValue)
    {
        switch (unitType)
        {
            case TemperatureUnit.Celsius:
                return celsiusValue;

            case TemperatureUnit.Fahrenheit:
                return (celsiusValue * 9.0 / 5.0) + 32.0;

            case TemperatureUnit.Kelvin:
                return celsiusValue + 273.15;

            default:
                throw new ArgumentException("Unsupported temperature unit");
        }
    }

    public string GetSymbol(TemperatureUnit unitType)
    {
        switch (unitType)
        {
            case TemperatureUnit.Celsius:
                return "°C";

            case TemperatureUnit.Fahrenheit:
                return "°F";

            case TemperatureUnit.Kelvin:
                return "K";

            default:
                return unitType.ToString();
        }
    }
}
}