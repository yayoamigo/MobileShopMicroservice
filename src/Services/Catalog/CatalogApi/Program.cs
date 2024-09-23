

using BuildingBlocks.Behaviours;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var assembly = typeof(Program).Assembly;

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidatorBehaviour<,>));
});


builder.Services.AddValidatorsFromAssembly(assembly);
builder.Services.AddCarter();

builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("Marten")!);
}).UseLightweightSessions();

var app = builder.Build();

app.MapCarter();
app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;
        if (exceptionHandlerPathFeature == null)
        {
            return;
        }

       var problemDetails = new ProblemDetails
        {
            Title = exceptionHandlerPathFeature.Message,
            Status = StatusCodes.Status500InternalServerError,
            Detail = exceptionHandlerPathFeature.StackTrace
        };

        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(exceptionHandlerPathFeature.Message, "An error occurred while processing your request");

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problemDetails);

    });

});

app.Run();
