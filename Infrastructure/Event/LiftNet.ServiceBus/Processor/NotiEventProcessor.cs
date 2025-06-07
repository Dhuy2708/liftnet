using LiftNet.Contract.Dtos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.ServiceBus.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.ServiceBus.Processor
{
    public class NotiEventProcessor : EventBaseProcessor
    {
        private readonly ILogger<NotiEventProcessor> _logger;

        public NotiEventProcessor(EventMessage eventMessage, IServiceProvider serviceProvider)
            : base(eventMessage, serviceProvider)
        {
            _logger = serviceProvider.GetRequiredService<ILogger<NotiEventProcessor>>();
        }

        protected override async Task HandleAsync(IServiceScope scope)
        {
            try
            {
                var notiService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                var notiMessage = JsonConvert.DeserializeObject<NotiMessageDto>(_eventMessage.Context);
                if (notiMessage == null)
                {
                    throw new Exception("noti message is null");
                }
                await notiService.Send(notiMessage, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
