using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace FluentValidation;

static class ValidateAttributeFilter
{
    sealed record ValidationDescriptor(
        int ArgumentIndex,
        Type ArgumentType,
        string? ArgumentName,
        Type ValidatorType
    );

    public static EndpointFilterDelegate Factory(
        EndpointFilterFactoryContext factoryContext,
        EndpointFilterDelegate next
    )
    {
        var validators = GetValidatorsDescriptors(factoryContext.MethodInfo);

        return async context =>
        {
            foreach (var descriptor in validators)
            {
                if (context.Arguments[descriptor.ArgumentIndex] is not { } argument)
                    continue;

                if (context.HttpContext.RequestServices.GetServices(descriptor.ValidatorType)
                        .FirstOrDefault() is not IValidator validator)
                    continue;

                var validationResult = await validator.ValidateAsync(
                    new ValidationContext<object>(argument)
                );

                if (validationResult.IsValid) continue;

                return TypedResults.ValidationProblem(
                    validationResult.ToDictionary(),
                    type: descriptor.ArgumentType.Name,
                    title:
                    $"One or more validation errors occurred ({descriptor.ArgumentName})"
                );
            }

            return await next(context);
        };
    }

    static IEnumerable<ValidationDescriptor> GetValidatorsDescriptors(MethodBase methodInfo)
    {
        var parameters = methodInfo.GetParameters();
        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            if (parameter.GetCustomAttribute<ValidateAttribute>() is null)
                continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(parameter.ParameterType);

            yield return new
            (
                ArgumentIndex: i,
                ArgumentType: parameter.ParameterType,
                ArgumentName: parameter.Name,
                ValidatorType: validatorType
            );
        }
    }
}
