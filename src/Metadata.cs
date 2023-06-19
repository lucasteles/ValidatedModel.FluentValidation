using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace FluentValidation;

sealed class ValidationProblemResponseMetadata : IProducesResponseTypeMetadata
{
    public Type? Type { get; } = typeof(ValidationProblemDetails);
    public int StatusCode => 400;
    public IEnumerable<string> ContentTypes { get; } = new[] { "application/problem+json" };
}

sealed class ParameterMetadata<T> : IAcceptsMetadata
{
    public IReadOnlyList<string> ContentTypes { get; }
    public Type? RequestType { get; }
    public bool IsOptional { get; }

    public ParameterMetadata(IReadOnlyList<string> contentTypes, bool isOptional)
    {
        var type = typeof(T);
        ContentTypes = contentTypes;
        RequestType = type;
        IsOptional = isOptional;
    }
}
