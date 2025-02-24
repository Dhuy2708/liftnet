using FluentValidation.Results;
using LiftNet.Domain.Exceptions;
using LiftNet.Domain.Response;
using Newtonsoft.Json;
using System.Net;

namespace LiftNet.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

            List<string> errors = [];
            List<ValidationFailure>? failures = null;

            if (exception is BaseException baseException)
            {
                errors = baseException.Errors;
            }

            switch (exception)
            {
                case BadRequestException badRequestException:
                    statusCode = HttpStatusCode.BadRequest;
                    failures = badRequestException.ValidationFailure;
                    break;
                case ValidationException validationException:
                    statusCode = HttpStatusCode.BadRequest;
                    break;
                case NotFoundException notFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    break;
                default:
                    break;
            }

            var result = LiftNetRes<object>.ErrorResponse(exception.Message, errors, failures);
            context.Response.StatusCode = (int)statusCode;
            return context.Response.WriteAsJsonAsync(result);
        }
    }
}
