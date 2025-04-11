namespace Thunders.TechTest.ApiService.API.Dtos
{
    public class ReportVehicleCountByTollStationRequestDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TollStation { get; set; }
    }
}