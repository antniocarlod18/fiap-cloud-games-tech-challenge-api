using FiapCloudGamesTechChallenge.Application.Dtos;
using FiapCloudGamesTechChallenge.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FiapCloudGamesTechChallenge.Application.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch(BaseException ex)
            {
                await HandleCustomExceptionResponseAsync(context, (int)ex.StatusCode,
                    ex.Message);
            }
            catch (Exception ex)
            {
                await HandleCustomExceptionResponseAsync(context, (int)HttpStatusCode.InternalServerError,
                    "Sorry! We ran into a problem. Please try again soon.");
            }
        }

        private async Task HandleCustomExceptionResponseAsync(HttpContext context, int statusCode, string? message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = new ErrorDto()
            {
                StatusCode = context.Response.StatusCode,
                Message = message
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }
    }
}
