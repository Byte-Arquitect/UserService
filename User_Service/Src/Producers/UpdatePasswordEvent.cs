using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Shared.Messages;

namespace User_Service.Src.Producers
{
    public class UpdatePasswordEvent 
    {
        private readonly IPublishEndpoint _publishEnpoint;

        public UpdatePasswordEvent(IPublishEndpoint publishEnpoint)
        {
            _publishEnpoint = publishEnpoint;
        }

        public async Task<IActionResult> PublishUpdateEvent(string userUuid, string newPassword){
            await _publishEnpoint.Publish<UpdatePasswordMessage>(new UpdatePasswordMessage
            {
                UserUuid = userUuid,
                NewPassword = newPassword
            });

            return new OkObjectResult("Evento enviado con éxito");
        }
    }
}