using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using User_Service.Src.Dtos;
using User_Service.Src.Messages;

namespace User_Service.Src.Producers
{
    public class RegisterEvent
    {
        private readonly IPublishEndpoint _publishEnpoint;

        public RegisterEvent(IPublishEndpoint publishEnpoint)
        {
            _publishEnpoint = publishEnpoint;
        }

        public async Task<IActionResult> PublishRegisterEvent(string Email, string Password, string userUuid)
        {
            var userEmail = Email;
            var userPass = Password;
            var userUUID = userUuid;

            await _publishEnpoint.Publish<RegisterUserMessage>(new
            {
                UserToLogin = new RegisterUserMessage
                {
                    Email = userEmail,
                    Password = userPass,
                    UserUuid = userUUID
                }
            });

            return new OkObjectResult("Tamos bien");
        }
    }
}
