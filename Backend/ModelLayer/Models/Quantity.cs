using System;
using ModelLayer.Interfaces;

namespace ModelLayer.Models
{
    public sealed class Quantity<TUnit> where TUnit : struct, Enum
    {
        private const double PrecisionLimit = 1e-6;

        private readonly IUnitConverter<TUnit> _converter;

        public double Value { get; }
        public TUnit Unit { get; }

        public Quantity(double amount, TUnit unitType, IUnitConverter<TUnit> converter)
        {
            if (!double.IsFinite(amount))
                throw new ArgumentException("Numeric value is not valid.");

            _converter = converter ?? throw new ArgumentNullException(nameof(converter));

            Value = amount;
            Unit = unitType;
        }

        private enum OperationType
        {
            Add,
            Subtract,
            Divide
        }

        private double ConvertCurrentToBase()
        {
            return _converter.ConvertToBase(Unit, Value);
        }

        private double ExecuteBaseOperation(Quantity<TUnit> other, OperationType op)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            double baseA = ConvertCurrentToBase();
            double baseB = other.ConvertCurrentToBase();

            switch (op)
            {
                case OperationType.Add:
                    return baseA + baseB;

                case OperationType.Subtract:
                    return baseA - baseB;

                case OperationType.Divide:
                    if (Math.Abs(baseB) < PrecisionLimit)
                        throw new ArithmeticException("Cannot divide by zero.");

                    return baseA / baseB;

                default:
                    throw new InvalidOperationException();
            }
        }

        public Quantity<TUnit> ConvertTo(TUnit destinationUnit)
        {
            double baseVal = ConvertCurrentToBase();
            double newValue = _converter.ConvertFromBase(destinationUnit, baseVal);

            return new Quantity<TUnit>(newValue, destinationUnit, _converter);
        }

        public Quantity<TUnit> Add(Quantity<TUnit> other, TUnit? resultUnit = null)
        {
            double baseResult = ExecuteBaseOperation(other, OperationType.Add);

            TUnit target = resultUnit ?? Unit;
            double converted = _converter.ConvertFromBase(target, baseResult);

            return new Quantity<TUnit>(converted, target, _converter);
        }

        public Quantity<TUnit> Subtract(Quantity<TUnit> other, TUnit? resultUnit = null)
        {
            double baseResult = ExecuteBaseOperation(other, OperationType.Subtract);

            TUnit target = resultUnit ?? Unit;
            double converted = _converter.ConvertFromBase(target, baseResult);

            return new Quantity<TUnit>(converted, target, _converter);
        }

        public double Divide(Quantity<TUnit> other)
        {
            return ExecuteBaseOperation(other, OperationType.Divide);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Quantity<TUnit> other)
                return false;

            return Math.Abs(ConvertCurrentToBase() - other.ConvertCurrentToBase()) < PrecisionLimit;
        }

        public override int GetHashCode()
        {
            return ConvertCurrentToBase().GetHashCode();
        }

        public override string ToString()
        {
            return $"{Value} {_converter.GetSymbol(Unit)}";
        }
    }
}