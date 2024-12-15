using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using User_Service.Src.Models;
using User_Service.Src.Protos;
using User_Service.Src.Services;
using User_Service.Src.Services.Interfaces;

namespace User_Service.Src.Controllers
{
    public class UserController : UserProto.UserProtoBase
    {
        private readonly ILogger<UserService> _logger;
        private readonly IUserService _userService;

        public UserController(ILogger<UserService> logger, IUserService userService)
        {
            _userService = userService;
            _logger = logger;
        }

        public override async Task<ResponseMyProgress> myProgress(
            Empty request,
            ServerCallContext context
        )
        {
            try
            {
                var userProgress = await _userService.GetUserProgress(context);
                return userProgress;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while getting user progress");
                throw;
            }
        }

        public override async Task<ResponseSetMyProgress> SetMyProgress(
            RequestSetMyProgress request,
            ServerCallContext context
        )
        {
            try
            {
                var response = await _userService.SetUserProgress(request, context);
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while setting user progress");
                throw;
            }
        }

        public override async Task<ResponseGetProfile> GetProfile(
            Empty request,
            ServerCallContext context
        )
        {
            try
            {
                var userProfile = await _userService.GetUserProfile(context);
                return userProfile;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while getting user profile");
                throw;
            }
        }
    }
}
