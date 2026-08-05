using IranValidator.Core.Results;

namespace IranValidator.Core;

/// <summary>
/// Defines a validator for values of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of value to validate.</typeparam>
public interface IValidator<in T>
{
    /// <summary>
    /// Validates the specified value.
    /// </summary>
    ValidationResult Validate(T value);
}
