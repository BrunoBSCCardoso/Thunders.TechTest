using Thunders.TechTest.ApiService.Events;
using Thunders.TechTest.ApiService.Interfaces;
using Thunders.TechTest.OutOfBox.Queues;

namespace Thunders.TechTest.ApiService.Producers
{
    public class TollStationUsageRegisteredProducer : ITollStationUsageRegisteredProducer
    {
        private readonly IMessageSender _messageSender;

        public TollStationUsageRegisteredProducer(IMessageSender messageSender)
        {
            _messageSender = messageSender;
        }

        public async Task RegisterAsync(TollStationUsageRegisteredEvent messageEvent)
        {
            await _messageSender.SendLocal(messageEvent);
        }
    }
}
