using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using User_Service.Src.Exceptions;
using System.Threading.Tasks;
using User_Service.Src.Common.Constants;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace User_Service.Src.Middleware
{
    public class ExceptionHandlingMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionHandlingMiddleware(RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IHostEnvironment env
        )
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                if (exceptionMapping.TryGetValue(ex.GetType(), out var mapping))
                {
                    await GenerateHttpResponse(ex, context, mapping.ErrorMessage, mapping.StatusCode);
                }
                else
                {
                    await GenerateHttpResponse(ex, context, ErrorMessages.InternalServerError, 500);
                }
            }
        }

        private readonly Dictionary<Type, (string ErrorMessage, int StatusCode)> exceptionMapping = new()
            {
                { typeof(InvalidCredentialException), (ErrorMessages.InvalidCredentials, 16) },
                { typeof(EntityNotFoundException), (ErrorMessages.EntityNotFound, 5) },
                { typeof(EntityDeletedException), (ErrorMessages.EntityNotFound, 5) },
                { typeof(InvalidJwtException), (ErrorMessages.InvalidCredentials, 16) }, 
                { typeof(DuplicateUserException), (ErrorMessages.DuplicateUser, 6) },
                { typeof(DisabledUserException), (ErrorMessages.DisabledUser, 7) },      
                { typeof(InternalErrorException), (ErrorMessages.InternalServerError, 13) },
                { typeof(UnauthorizedAccessException), (ErrorMessages.UnauthorizedAccess, 7) },
                { typeof(DuplicateEntityException), (ErrorMessages.EntityDuplicated, 6) }
            };

        private async Task GenerateHttpResponse(
            Exception ex,
            HttpContext context,
            string errorTitle,
            int statusCode
        )
        {
            if (statusCode == 500)
                _logger.LogError(ex, ex.Message);
            else
                _logger.LogInformation(ex, ex.Message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = new ProblemDetails
            {
                Status = statusCode,
                Title = errorTitle,
                Detail = ex.Message,
                Instance = context.Request.Path, // Internal API URL that caused the error
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            var json = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(json);
        }
    

    }
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandling(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}