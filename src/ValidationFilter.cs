using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FluentValidation.ValidatedModel;

sealed class ValidatedModelFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next
    )
    {
        var validatable =
            context.Arguments.FirstOrDefault(x => x is IValidatedModel) as IValidatedModel;
        if (validatable is null or { IsValid: true })
            return await next(context);

        return TypedResults.ValidationProblem(validatable.Errors);
    }
}
