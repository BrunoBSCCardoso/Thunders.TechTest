using Thunders.TechTest.ApiService.DataBase.Models;

namespace Thunders.TechTest.ApiService.API.Dtos
{
    public class ReportVehicleTypeCountByStationResponseDto
    {
        public VehicleType VehicleType { get; set; }
        public int Amount { get; set; }
    }
}