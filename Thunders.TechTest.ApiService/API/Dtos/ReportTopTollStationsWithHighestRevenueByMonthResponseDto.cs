namespace Thunders.TechTest.ApiService.API.Dtos
{
    public class ReportTopTollStationsWithHighestRevenueByMonthResponseDto
    {
        public string TollStation { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}