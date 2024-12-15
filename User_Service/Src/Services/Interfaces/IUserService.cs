using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using User_Service.Src.Protos;

namespace User_Service.Src.Services.Interfaces
{
    public interface IUserService
    {
        public Task<ResponseMyProgress> GetUserProgress(ServerCallContext context);
        public Task<ResponseSetMyProgress> SetUserProgress(
            RequestSetMyProgress request,
            ServerCallContext context
        );
        public Task<ResponseGetProfile> GetUserProfile(ServerCallContext context);
    }
}
