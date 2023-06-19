using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Metadata;

namespace FluentValidation;

/// <summary>
/// Skips auto validation for the parameter this attribute is applied
/// </summary>
[Serializable]
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class ManualValidationAttribute : Attribute { }

/// <summary>
/// Automatically validate argument with registered fluent validations
/// </summary>
[Serializable]
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class ValidateAttribute : Attribute { }
