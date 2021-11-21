using Microsoft.AspNetCore.Http;
using Order.Service.Exceptions;
using Order.WebAPI.ViewModels;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Order.WebAPI.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(InvalidApiParameterException ex)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = (int)HttpStatusCode.BadRequest;

                var result = JsonSerializer.Serialize(new BadRequestResponse 
                { 
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = "One or more validation errors occurred.",
                    Status = (int)HttpStatusCode.BadRequest,
                    Errors = ex.Errors.Errors
                });

                await response.WriteAsync(result);
            }
            catch(ApiNotFoundException ex)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = (int)HttpStatusCode.NotFound;

                var result = JsonSerializer.Serialize(new ServerErrorResponse(ex.Message));
                await response.WriteAsync(result);
            }
            catch (Exception)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var result = JsonSerializer.Serialize(new ServerErrorResponse("Internal Server Error"));
                await response.WriteAsync(result);
            }
        }
    }
}
