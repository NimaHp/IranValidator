using IranValidator.Core;
using IranValidator.Core.Validators;

namespace IranValidator.DataAnnotations;

/// <summary>
/// Validates that a string property is a valid Iranian Economic Code (کد اقتصادی).
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class IranEconomicCodeAttribute : IranValidationAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IranEconomicCodeAttribute"/> class.
    /// </summary>
    public IranEconomicCodeAttribute()
    {
    }

    /// <inheritdoc />
    protected override IStringValidator Validator => EconomicCodeValidator.Instance;
}
