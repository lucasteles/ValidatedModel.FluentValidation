namespace FluentValidation;

/// <summary>
/// Fluent Validation Filter Options
/// </summary>
public sealed class FluentValidationFilterOptions
{
    /// <summary>
    /// Automatically validate and return validations problems for Validaded[T] parameters
    /// </summary>
    public bool AutoValidated { get; set; } = true;
}
