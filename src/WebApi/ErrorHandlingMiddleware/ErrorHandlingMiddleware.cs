using Application.Exceptions;
using Common.Responses.Wrappers;
using System.Net;
using System.Text.Json;

namespace WebApi.ErrorHandlingMiddleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ErrorHandlingMiddleware(RequestDelegate next)
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
                var response = httpContext.Response;
                response.ContentType = "application/json";

                var responseWrapper = await ResponseWrapper.FailAsync(ex.Message);

                switch (ex)
                {
                    case CustomValidationException vex:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    default:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                var result = JsonSerializer.Serialize(responseWrapper);
                await response.WriteAsync(result);
            }
        }
    }
}
