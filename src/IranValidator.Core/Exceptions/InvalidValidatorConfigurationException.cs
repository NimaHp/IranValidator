namespace IranValidator.Core.Exceptions;

/// <summary>
/// Exception thrown when a validator is configured incorrectly.
/// This is for programming errors, not invalid input.
/// </summary>
/// <remarks>Kept for backward compatibility — no validator currently throws this; reserved for future configuration validation.</remarks>
public sealed class InvalidValidatorConfigurationException : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="InvalidValidatorConfigurationException"/>.
    /// </summary>
    public InvalidValidatorConfigurationException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of <see cref="InvalidValidatorConfigurationException"/> with an inner exception.
    /// </summary>
    public InvalidValidatorConfigurationException(string message, Exception innerException)
        : base(message, innerException) { }
}
