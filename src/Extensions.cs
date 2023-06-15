using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace FluentValidation.ValidatedModel;

/// <summary>
/// FluentValidation.ValidatedModel extensions
/// </summary>
public static class FluentValidationValidatorExtensions
{
    /// <summary>
    /// Add FluentValidation filter to return Validation problems
    /// </summary>
    /// <param name="builder"></param>
    /// <typeparam name="TBuilder"></typeparam>
    public static TBuilder AddFluentValidationFilter<TBuilder>(this TBuilder builder)
        where TBuilder : IEndpointConventionBuilder
    {
        builder.AddEndpointFilter(new ValidatedModelFilter());
        return builder;
    }
}
