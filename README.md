[![CI](https://github.com/lucasteles/FluentValidation.ValidatedModel/actions/workflows/ci.yml/badge.svg)](https://github.com/lucasteles/FluentValidation.ValidatedModel/actions/workflows/ci.yml)
[![Nuget](https://img.shields.io/nuget/v/FluentValidation.ValidatedModel.svg?style=flat)](https://www.nuget.org/packages/FluentValidation.ValidatedModel)
![](https://raw.githubusercontent.com/lucasteles/FluentValidation.ValidatedModel/badges/badge_linecoverage.svg)
![](https://raw.githubusercontent.com/lucasteles/FluentValidation.ValidatedModel/badges/badge_branchcoverage.svg)
![](https://raw.githubusercontent.com/lucasteles/FluentValidation.ValidatedModel/badges/test_report_badge.svg)
![](https://raw.githubusercontent.com/lucasteles/FluentValidation.ValidatedModel/badges/lines_badge.svg)

![](https://raw.githubusercontent.com/lucasteles/FluentValidation.ValidatedModel/badges/dotnet_version_badge.svg)
![](https://img.shields.io/badge/Lang-C%23-green)

# FluentValidation.ValidatedModel

Automatically bind and validate models with [FluentValidation](https://github.com/FluentValidation/FluentValidation) on
ASP.NET

## Getting started

[NuGet package](https://www.nuget.org/packages/FluentValidation.ValidatedModel) available:

```ps
$ dotnet add package FluentValidation.ValidatedModel
```

## How To Use:

```csharp
using FluentValidation;
using FluentValidation.ValidatedModel;

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
```

