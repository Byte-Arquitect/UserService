using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using User_Service.Src.Dtos;
using Shared.Messages;

namespace User_Service.Src.Producers
{
    public class RegisterEvent
    {
        private readonly IPublishEndpoint _publishEnpoint;

        public RegisterEvent(IPublishEndpoint publishEnpoint)
        {
            _publishEnpoint = publishEnpoint;
        }

        public async Task<IActionResult> PublishRegisterEvent(string email, string password, string userUuid)
        {
            await _publishEnpoint.Publish<RegisterUserMessage>(new RegisterUserMessage
            {
                    Email = email,
                    Password = password,
                    UserUuid = userUuid
            });

            return new OkObjectResult("Evento enviado con éxito");
        }
    }
}
