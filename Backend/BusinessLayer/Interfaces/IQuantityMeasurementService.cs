using System;
using ModelLayer.Models;
using ModelLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    /// <summary>
    /// Defines operations for working with Quantity objects.
    /// </summary>
    public interface IQuantityMeasurementService
    {
        QuantityResultDto Add(AddRequestDto request);
        QuantityResultDto Subtract(SubtractRequestDto request);
        DivisionResultDto Divide(DivideRequestDto request);
        ComparisonResultDto Compare(ComparisonRequestDto request);
        QuantityResultDto Convert(ConversionRequestDto request);
        ComparisonResultDto Compare<U>(Quantity<U> firstQuantity, Quantity<U> secondQuantity)
            where U : struct, Enum;

        QuantityResultDto DemonstrateConversion<U>(double numericValue, U sourceType, U targetType)
            where U : struct, Enum;

        QuantityResultDto DemonstrateConversion<U>(Quantity<U> originalQuantity, U desiredUnit)
            where U : struct, Enum;

        QuantityResultDto DemonstrateAddition<U>(Quantity<U> leftOperand, Quantity<U> rightOperand)
            where U : struct, Enum;

        QuantityResultDto DemonstrateAddition<U>(Quantity<U> leftOperand, Quantity<U> rightOperand, U resultUnit)
            where U : struct, Enum;

        QuantityResultDto Subtract<U>(Quantity<U> firstValue, Quantity<U> secondValue, U resultUnit)
            where U : struct, Enum;

        DivisionResultDto Divide<T>(Quantity<T> dividend, Quantity<T> divisor)
            where T : struct, Enum;
    }
}