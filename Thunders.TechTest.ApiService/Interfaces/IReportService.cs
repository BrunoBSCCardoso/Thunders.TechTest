using Thunders.TechTest.ApiService.API.Dtos;

namespace Thunders.TechTest.ApiService.Interfaces
{
    public interface IReportService
    {
        /// <summary>
        /// Report to check the total value of toll usage per hour and per city
        /// </summary>
        /// <param name="reportInputDto"> Filtered by city name or empty to get all the cities </param>
        /// <returns> Report containing the time of use, the city and the total </returns>
        Task<IList<ReportTotalAmountPaidByHourAndCityResponseDto>> GetTotalByHourAndCityAsync(ReportTotalAmountPaidByHourAndCityRequestDto reportInputDto);

        /// <summary>
        /// Report of the number of vehicles that passed through a certain toll station
        /// </summary>
        /// <param name="tollStation"> Toll station name </param>
        /// <returns> Report containing date, vehicle and amount </returns>
        Task<IList<ReportVehicleTypeCountByStationResponseDto>> GetVehicleTypeCountByStationAsync(string tollStation, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Report that checks the toll stations that invoiced the most according to a range of dates
        /// </summary>
        /// <param name="requestDto"></param>
        /// <returns> Report containing TollStation and TotalRevenue</returns>
        Task<IList<ReportTopTollStationsWithHighestRevenueByMonthResponseDto>> GetTopTollStationsWithHighestRevenueAsync(ReportTopTollStationsWithHighestRevenueByMonthRequestDto requestDto);
    }
}
