
namespace Thunders.TechTest.Abstractions.Events
{
    public class TollStationUsageRegisteredEvent
    {
        public Guid EventId { get; set; } = Guid.NewGuid();
        public DateTime Timestamp { get; set; }
        public string StationName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public decimal AmountPaid { get; set; }
        public int VehicleType { get; set; }

    }

    public static class Validator
    {
        public static IEnumerable<string> Validate(this TollStationUsageRegisteredEvent eventMessage)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(eventMessage.StationName))
            {
                errors.Add("The Station name is null or empty.");
            }

            if (string.IsNullOrEmpty(eventMessage.City))
            {
                errors.Add("The city is null or empty.");
            }

            if (string.IsNullOrEmpty(eventMessage.State))
            {
                errors.Add("The state is null or empty.");
            }

            if (eventMessage.AmountPaid <= 0)
            {
                errors.Add("The Amount paid is invalid.");
            }

            return errors;
        }
    }

}