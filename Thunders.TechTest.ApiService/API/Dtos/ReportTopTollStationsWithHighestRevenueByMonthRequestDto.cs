namespace Thunders.TechTest.ApiService.API.Dtos
{
    public class ReportTopTollStationsWithHighestRevenueByMonthRequestDto
    {
        public int NumberOfTollStations { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}