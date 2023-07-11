﻿using ExtraDry.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using System.Net;

namespace ExtraDry.Server;

public static class ProblemDetailsResponse {

    internal static void RewriteResponse(ExceptionContext context, HttpStatusCode code, string? details = null) => RewriteResponse(context.HttpContext, context.Exception.GetType().Name, (int)code, context.Exception.Message, details);

    internal static void RewriteResponse(HttpContext httpContext, HttpStatusCode httpStatusCode, string details) => RewriteResponse(httpContext, httpStatusCode.ToString().ToLower(), httpContext.Response.StatusCode, httpStatusCode.ToString(), details: details);

    private static void RewriteResponse(HttpContext httpContext, string problem, int code, string title, string? details = null)
    {
        var problemDetails = CreateProblemDetails(httpContext, problem, code, title, details);
        var body = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions(JsonSerializerDefaults.Web) { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault});
        var response = httpContext.Response;
        response.Clear();
        response.StatusCode = code;
        response.ContentType = "application/problem+json";
        response.ContentLength = body.Length;
        response.WriteAsync(body);
    }

    private static ProblemDetails CreateProblemDetails(HttpContext httpContext, string problem, int code, string title, string? details = null)
    {
        var problemDetails = new ProblemDetails {
            Type = $"https://{httpContext.Request.Host}/problems/{DataConverter.CamelCaseToKebabCase(problem)}",
            Status = code,
            Title = title,
            Detail = details,
            Instance = httpContext.Request.Path
        };
        /*
         *  Adapted from DefaultProblemDetailsFactory (https://github.com/dotnet/aspnetcore/blob/2cb12687b20bc708d48f61a030682e8d5f12683f/src/Mvc/Mvc.Core/src/Infrastructure/DefaultProblemDetailsFactory.cs#L92C76-L92C76).
         *  ProblemDetails can't be shared between server and blazor therefore
         *  the ProblemDetailsFactory can't be used, which would usually populate
         *  the traceId. Given this the logic has been reproduced here.
         */
        var traceId = Activity.Current?.Id ?? httpContext?.TraceIdentifier;
        if(traceId != null) {
            problemDetails.Extensions["traceId"] = traceId;
        }
        return problemDetails;
    }
}
