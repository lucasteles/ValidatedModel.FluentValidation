using System;

namespace FluentValidation;

/// <summary>
/// Skips auto validation for the parameter this attribute is applied
/// </summary>
[Serializable]
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class ManualValidationAttribute : Attribute { }

/// <summary>
/// Automatically validate argument with registered fluent validations
/// </summary>
[Serializable]
[AttributeUsage(AttributeTargets.Parameter)]
public class ValidateAttribute : Attribute { }
