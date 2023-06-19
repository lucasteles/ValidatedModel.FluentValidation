using System.Text.Json;
using FluentValidation;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http.HttpResults;
using static Microsoft.AspNetCore.Http.TypedResults;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddValidatorsFromAssemblyContaining<PersonValidator>()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddFluentValidationRulesToSwagger();

var app = builder.Build();
app.UseSwagger().UseSwaggerUI();

var api = app.MapGroup("api")
    // Automatically returns validation problems for failed Validated<T> parameters
    .AddFluentValidationFilter();

api.MapPost("/person", (Validated<Person> person) =>
    Ok($"Hello {person.Value.Name}"));


api.MapPost("/person-simple", ([Validate] Person person) =>
    Ok($"Hello {person.Name}"));

api.MapPost("/person-manual",
    Results<Ok<string>, BadRequest> (
        ILogger<Program> logger,
        [ManualValidation] Validated<Person> person
    ) =>
    {
        if (!person.IsValid)
        {
            logger.LogInformation("Validation: {Errors}", JsonSerializer.Serialize(person.Errors));
            return BadRequest();
        }

        return Ok($"Hello {person.Value.Name}");
    });

app.Run();

public class Person
{
    public required string Name { get; init; }
    public required int Age { get; init; }
}

public class PersonValidator : AbstractValidator<Person>
{
    public PersonValidator()
    {
        RuleFor(p => p.Name).MinimumLength(3);
        RuleFor(p => p.Age).GreaterThanOrEqualTo(18);
    }
}
