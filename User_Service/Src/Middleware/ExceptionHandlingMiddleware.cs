using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;
using User_Service.Src.Common.Constants;
using User_Service.Src.Exceptions;

namespace User_Service.Src.Middleware
{
    public class ExceptionHandlingInterceptor : Interceptor
    {
        private readonly ILogger<ExceptionHandlingInterceptor> _logger;

        private readonly Dictionary<Type, (string ErrorMessage, StatusCode StatusCode)> _exceptionMapping = new()
        {
            { typeof(InvalidCredentialException), (ErrorMessages.InvalidCredentials, StatusCode.Unauthenticated) },
            { typeof(EntityNotFoundException), (ErrorMessages.EntityNotFound, StatusCode.NotFound) },
            { typeof(EntityDeletedException), (ErrorMessages.EntityNotFound, StatusCode.NotFound) },
            { typeof(InvalidJwtException), (ErrorMessages.InvalidCredentials, StatusCode.Unauthenticated) },
            { typeof(DuplicateUserException), (ErrorMessages.DuplicateUser, StatusCode.AlreadyExists) },
            { typeof(DisabledUserException), (ErrorMessages.DisabledUser, StatusCode.PermissionDenied) },
            { typeof(InternalErrorException), (ErrorMessages.InternalServerError, StatusCode.Internal) },
            { typeof(UnauthorizedAccessException), (ErrorMessages.InternalServerError, StatusCode.PermissionDenied) },
            { typeof(DuplicateEntityException), (ErrorMessages.EntityDuplicated, StatusCode.AlreadyExists) }
        };

        public ExceptionHandlingInterceptor(ILogger<ExceptionHandlingInterceptor> logger)
        {
            _logger = logger;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return await continuation(request, context);
            }
            catch (Exception ex) when (_exceptionMapping.TryGetValue(ex.GetType(), out var mapping))
            {
                _logger.LogWarning(ex, ex.Message);
                throw new RpcException(new Status(mapping.StatusCode, mapping.ErrorMessage));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new RpcException(new Status(StatusCode.Internal, "Internal Server Error"));
            }
        }
    }
}
