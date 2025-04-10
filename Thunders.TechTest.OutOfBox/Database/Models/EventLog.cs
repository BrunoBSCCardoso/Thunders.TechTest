namespace Thunders.TechTest.ApiService.DataBase.Models
{
    public class EventLog
    {
        public Guid EventId { get; set; }
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
        public string? Payload { get; set; }
    }

}