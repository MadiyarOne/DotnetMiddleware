using System.ComponentModel.DataAnnotations;
using DotnetMiddleware.Middlewares;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


app.UseMiddleware<CustomExceptionHandlerMiddleware>();

app.MapGet("/", () => "Some info");

app.MapGet("/file", () =>
{
    throw new FileNotFoundException("file not found");
});

app.MapGet("/validation", () =>
{
    throw new ValidationException("not valid path");
});
app.Run();