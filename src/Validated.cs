using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace FluentValidation;

/// <summary>
/// Used on validation in validation filter
/// </summary>
public interface IValidatedModel
{
    /// <summary>
    /// A collection of errors
    /// </summary>
    IDictionary<string, string[]> Errors { get; }

    /// <summary>
    /// Whether validation succeeded
    /// </summary>
    bool IsValid { get; }


    /// <summary>
    /// Model Type
    /// </summary>
    Type ModelType { get; }
}

/// <summary>
/// Auto validator for T
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class Validated<T> :
    IValidatedModel,
    IEndpointMetadataProvider,
    IEndpointParameterMetadataProvider
{
    /// <summary>
    /// Validated model
    /// </summary>
    public T Value { get; }

    /// <inheritdoc />
    public bool IsValid { get; }

    /// <inheritdoc />
    public Type ModelType { get; } = typeof(T);

    /// <inheritdoc />
    public IDictionary<string, string[]> Errors { get; }

    /// <summary>
    /// Implicit cast to T
    /// </summary>
    public static implicit operator T(Validated<T> validated) => validated.Value;

    Validated(T value, bool isValid, IDictionary<string, string[]> errors)
    {
        Value = value;
        IsValid = isValid;
        Errors = errors;
    }

    /// <summary>
    /// Aspnet core binding
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public static async ValueTask<Validated<T>> BindAsync(
        HttpContext context,
        ParameterInfo parameter)
    {
        ArgumentNullException.ThrowIfNull(context);
        var model = await context.Request.ReadFromJsonAsync<T>();
        if (model is null) return null!;

        var allValidators = context.RequestServices
            .GetRequiredService<IEnumerable<IValidator<T>>>()
            .ToArray();

        var validator = allValidators switch
        {
            [] => throw new InvalidOperationException(
                $"No registered IValidator<> for {typeof(T).FullName}"),
            [var single] => single,
            _ => allValidators.CombineValidators(),
        };

        var result = await validator.ValidateAsync(model);
        return new(model, result.IsValid, result.ToDictionary());
    }

    /// <inheritdoc />
    public static void PopulateMetadata(ParameterInfo parameter, EndpointBuilder builder)
    {
        if (builder.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault() is not { } httpMeta
            || httpMeta.HttpMethods.Contains(HttpMethods.Get))
            return;

        var accepts = builder.Metadata.OfType<IAcceptsMetadata>()
            .SelectMany(x => x.ContentTypes)
            .Distinct()
            .DefaultIfEmpty(MediaTypeNames.Application.Json)
            .ToArray();

        builder.Metadata.Add(new ParameterMetadata<T>(accepts, parameter.IsOptionalOrNullable()));
    }

    /// <inheritdoc />
    public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder) =>
        builder.Metadata.Add(new ValidationProblemResponseMetadata());
}
