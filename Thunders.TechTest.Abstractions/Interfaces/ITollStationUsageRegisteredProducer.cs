using Thunders.TechTest.Abstractions.Events;

namespace Thunders.TechTest.Abstractions.Interfaces
{
    public interface ITollStationUsageRegisteredProducer
    {
        Task Register(TollStationUsageRegisteredEvent messageEvent);
    }
}
