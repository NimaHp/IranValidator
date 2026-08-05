using IranValidator.Core;
using IranValidator.Core.Validators;

namespace IranValidator.DataAnnotations;

/// <summary>
/// Validates that a string property is a valid Iranian Vehicle Plate (پلاک خودرو).
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class IranVehiclePlateAttribute : IranValidationAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IranVehiclePlateAttribute"/> class.
    /// </summary>
    public IranVehiclePlateAttribute()
    {
    }

    /// <inheritdoc />
    protected override IStringValidator Validator => VehiclePlateValidator.Instance;
}
