using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace DotnetMiddleware.Middlewares;

public class CustomExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public CustomExceptionHandlerMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            await HandleExceptionAsync(context, e);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var result = string.Empty;

        switch (exception)
        {
            case ValidationException validationException:
                code = HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(new {From = "FTP server", Message = validationException.ValidationResult.ErrorMessage});
                break;
            case FileNotFoundException notFoundException:
                code = HttpStatusCode.NotFound;
                result = JsonSerializer.Serialize(new {From = "From validation server", notFoundException.Message});
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int) code;

        if (result == String.Empty)
        {
            result = JsonSerializer.Serialize(new {error = exception.Message});
        }

        return context.Response.WriteAsync(result);
    }
}