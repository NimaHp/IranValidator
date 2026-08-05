using IranValidator.Core;
using IranValidator.Core.Validators;

namespace IranValidator.DataAnnotations;

/// <summary>
/// Validates that a string property is a valid Iranian Company ID (شناسه ملی شرکت).
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class IranCompanyIdAttribute : IranValidationAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IranCompanyIdAttribute"/> class.
    /// </summary>
    public IranCompanyIdAttribute()
    {
    }

    /// <inheritdoc />
    protected override IStringValidator Validator => CompanyIdValidator.Instance;
}
