using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace FluentValidation;

static class ValidatedModelFilter
{
    public static EndpointFilterDelegate Factory(
        FluentValidationFilterOptions options,
        EndpointFilterFactoryContext factoryContext,
        EndpointFilterDelegate next
    )
    {
        var validatableIndexes = GetValidatableIndexes(factoryContext.MethodInfo)
            .ToArray().AsReadOnly();

        return async context =>
        {
            if (!options.AutoValidated)
                return await next(context);

            foreach (var (index, name) in validatableIndexes)
            {
                if (context.Arguments[index] is not IValidatedModel validatable
                    || validatable.IsValid)
                    continue;

                return TypedResults.ValidationProblem(
                    validatable.Errors,
                    type: validatable.ModelType.Name,
                    title: $"One or more validation errors occurred ({name})"
                );
            }

            return await next(context);
        };
    }

    static IEnumerable<(int, string?)> GetValidatableIndexes(MethodBase methodInfo)
    {
        var parameters = methodInfo.GetParameters();
        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];

            if (!parameter.ParameterType.IsAssignableTo(typeof(IValidatedModel)))
                continue;

            if (parameter.GetCustomAttribute<ManualValidationAttribute>() is not null)
                continue;

            yield return (i, parameter.Name);
        }
    }
}
