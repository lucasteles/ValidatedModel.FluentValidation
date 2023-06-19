using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace FluentValidation;

/// <summary>
/// ValidatedModel.FluentValidation extensions
/// </summary>
public static class FluentValidationValidatorExtensions
{
    /// <summary>
    /// Add FluentValidation filter to return Validation problems
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="options"></param>
    /// <typeparam name="TBuilder"></typeparam>
    public static TBuilder AddFluentValidationFilter<TBuilder>(
        this TBuilder builder,
        FluentValidationFilterOptions options)
        where TBuilder : IEndpointConventionBuilder
    {
        builder.AddEndpointFilterFactory(ValidateAttributeFilter.Factory);
        builder.AddEndpointFilterFactory((context, next) =>
            ValidatedModelFilter.Factory(options, context, next));

        return builder;
    }

    /// <summary>
    /// Add FluentValidation filter to return Validation problems
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configure"></param>
    /// <typeparam name="TBuilder"></typeparam>
    public static TBuilder AddFluentValidationFilter<TBuilder>(
        this TBuilder builder,
        Action<FluentValidationFilterOptions> configure)
        where TBuilder : IEndpointConventionBuilder
    {
        FluentValidationFilterOptions options = new();
        configure(options);
        return builder.AddFluentValidationFilter(options);
    }

    /// <summary>
    /// Add FluentValidation filter to return Validation problems
    /// </summary>
    /// <param name="builder"></param>
    /// <typeparam name="TBuilder"></typeparam>
    public static TBuilder AddFluentValidationFilter<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder =>
        builder.AddFluentValidationFilter(new FluentValidationFilterOptions());

    internal static IValidator<T> CombineValidators<T>(
        this IEnumerable<IValidator<T>> allValidators
    )
    {
        InlineValidator<T> validator = new();
        foreach (var v in allValidators) validator.Include(v);
        return validator;
    }

    internal static bool IsOptionalOrNullable(this ParameterInfo parameter)
    {
        if (parameter.IsOptional) return true;

        if (parameter.ParameterType.IsValueType)
            return Nullable.GetUnderlyingType(parameter.ParameterType) != null;

        NullabilityInfoContext nullabilityContext = new();
        var nullabilityInfo = nullabilityContext.Create(parameter);

        return nullabilityInfo.WriteState is NullabilityState.Nullable;
    }
}
