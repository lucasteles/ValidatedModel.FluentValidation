[![CI](https://github.com/lucasteles/ValidatedModel.FluentValidation/actions/workflows/ci.yml/badge.svg)](https://github.com/lucasteles/ValidatedModel.FluentValidation/actions/workflows/ci.yml)
[![Nuget](https://img.shields.io/nuget/v/ValidatedModel.FluentValidation.svg?style=flat)](https://www.nuget.org/packages/ValidatedModel.FluentValidation)
![](https://raw.githubusercontent.com/lucasteles/ValidatedModel.FluentValidation/badges/badge_linecoverage.svg)
![](https://raw.githubusercontent.com/lucasteles/ValidatedModel.FluentValidation/badges/badge_branchcoverage.svg)
![](https://raw.githubusercontent.com/lucasteles/ValidatedModel.FluentValidation/badges/test_report_badge.svg)
![](https://raw.githubusercontent.com/lucasteles/ValidatedModel.FluentValidation/badges/lines_badge.svg)

![](https://raw.githubusercontent.com/lucasteles/ValidatedModel.FluentValidation/badges/dotnet_version_badge.svg)
![](https://img.shields.io/badge/Lang-C%23-green)

# ValidatedModel.FluentValidation

Automatically bind and validate models with [FluentValidation](https://github.com/FluentValidation/FluentValidation) on
ASP.NET

## Getting started

[NuGet package](https://www.nuget.org/packages/ValidatedModel.FluentValidation) available:

```ps
$ dotnet add package ValidatedModel.FluentValidation
```

## How To Use:

```csharp
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidatorsFromAssemblyContaining<PersonValidator>();

var app = builder.Build();
var api = app.MapGroup("api")
    // Enable validation filter
    // Automatically validate Validated<T> parameters
    .AddFluentValidationFilter();

// Returns validation problems if validation fail
api.MapPost("/person", (Validated<Person> person) => $"Hello {person.Value.Name}");

// using attribute only
api.MapPost("/person-simple", ([Validate] Person person) => $"Hello {person.Name}");

// Skipping auto validation
api.MapPost("/person-manual", (ILogger<Program> logger, [ManualValidation] Validated<Person> person) =>
{
    if (!person.IsValid)
    {
        logger.LogInformation("Validation: {Errors}", JsonSerializer.Serialize(person.Errors));
        return Results.BadRequest();
    }
    return $"Hello {person.Value.Name}";
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
        RuleFor(p => p.Name).NotEmpty();
        RuleFor(p => p.Age).GreaterThanOrEqualTo(18);
    }
}
```

