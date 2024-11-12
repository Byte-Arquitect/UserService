using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using User_Service.Src.Protos;
using Google.Protobuf.Collections;

namespace User_Service.Src.Services
{
    public class TestService : Test.TestBase
    {
        private readonly ILogger<TestService> _logger;

        public TestService(ILogger<TestService> logger)
        {
            _logger = logger;
        }
        
        public override Task<Response> SayHello(Request request, ServerCallContext context){
            return Task.FromResult(new Response
            {
                Message = "Hello " + request.Nombre
            });
        }
    }

}