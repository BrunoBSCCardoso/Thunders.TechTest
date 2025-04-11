namespace Thunders.TechTest.ApiService.API.Dtos
{
    public class ReportTotalAmountPaidByHourAndCityResponseDto
    {
        public DateTime TimeStamp { get; set; }
        public string City { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}