namespace Thunders.TechTest.ApiService.DataBase.Models
{
    public class TollStationUsage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Timestamp { get; set; }
        public string StationName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public decimal AmountPaid { get; set; }
        public VehicleType VehicleType { get; set; }
    }
}