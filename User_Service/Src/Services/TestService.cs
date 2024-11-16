using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using User_Service.Src.Protos;
using Google.Protobuf.Collections;
using User_Service.Src.Exceptions;

namespace User_Service.Src.Services
{
    public class TestService : Test.TestBase
    {
        private readonly ILogger<TestService> _logger;

        public TestService(ILogger<TestService> logger)
        {
            _logger = logger;
        }
        
        public override Task<Response> SayHello(Request request, ServerCallContext context)
{
            // Simular una condición de "entidad no encontrada"
             if (string.IsNullOrEmpty(request.Nombre))
            {
                throw new EntityNotFoundException("El nombre especificado no fue encontrado.");
            }

            // Si el nombre es válido, devolver la respuesta normal
            return Task.FromResult(new Response
            {
                Message = "Hello " + request.Nombre
            });
        }
    }

}