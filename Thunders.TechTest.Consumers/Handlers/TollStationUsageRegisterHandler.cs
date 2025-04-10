using Rebus.Handlers;
using System.Text.Json;
using Thunders.TechTest.Abstractions.Events;
using Thunders.TechTest.ApiService.DataBase.Context;
using Thunders.TechTest.ApiService.DataBase.Models;

namespace Thunders.TechTest.Consumers.Handlers
{

    public class TollStationUsageRegisterHandler : IHandleMessages<TollStationUsageRegisteredEvent>
    {
        private readonly ThunderDbContext _dbContext;

        public TollStationUsageRegisterHandler(ThunderDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(TollStationUsageRegisteredEvent eventMessage)
        {
            IEnumerable<string> errors = eventMessage.Validate();

            if (errors.Any())
            {
                _dbContext.EventLogs.Add(new EventLog 
                { 
                    EventId = eventMessage.EventId,
                    Status = "Failed",
                    ErrorMessage = "Validation errors were found in business rules",
                    Payload = string.Join(", ", errors)
                });
            }
            else
            {
                _dbContext.TollStationUsages.Add(new TollStationUsage
                {
                    Timestamp = eventMessage.Timestamp,
                    StationName = eventMessage.StationName,
                    City = eventMessage.City,
                    State = eventMessage.State,
                    AmountPaid = eventMessage.AmountPaid,
                    VehicleType = (VehicleType)eventMessage.VehicleType
                });

                string eventSerializer = JsonSerializer.Serialize(eventMessage);

                _dbContext.EventLogs.Add(new EventLog
                {
                    EventId = eventMessage.EventId,
                    Status = "Successfully",
                    Payload = eventSerializer
                });
            }

            await _dbContext.SaveChangesAsync();

        }


    }
}