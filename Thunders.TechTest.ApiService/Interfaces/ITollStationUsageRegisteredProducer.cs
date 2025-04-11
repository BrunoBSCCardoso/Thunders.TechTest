using Thunders.TechTest.ApiService.Events;

namespace Thunders.TechTest.ApiService.Interfaces
{
    public interface ITollStationUsageRegisteredProducer
    {
        Task RegisterAsync(TollStationUsageRegisteredEvent messageEvent);
    }
}
