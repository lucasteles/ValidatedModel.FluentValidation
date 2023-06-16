using FluentValidation;
using ValidatedModel.FluentValidation;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidatorsFromAssemblyContaining<PersonValidator>();

var app = builder.Build();
var api = app.MapGroup("api")
    // Automatically returns validation problems for failed Validated<T> parameters
    .AddFluentValidationFilter();

api.MapPost("/person", (Validated<Person> person) => $"Hello {person.Value.Name}");

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
        RuleFor(p => p.Name).NotEmpty();
        RuleFor(p => p.Age).GreaterThanOrEqualTo(18);
    }
}


