using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Thunders.TechTest.ApiService.API.Dtos;
using Thunders.TechTest.ApiService.Interfaces;
using Thunders.TechTest.ApiService.Services;

namespace Thunders.TechTest.ApiService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly ILogger<TollStationRegisterController> _logger;
        private readonly IReportService _service;

        public ReportsController(ILogger<TollStationRegisterController> logger, IReportService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet("report-total-amount-paid-by-hour-and-city")]
        public async Task<IActionResult> GetTotalAmoutPaidByHourdAndCityAsync([FromQuery]ReportTotalAmountPaidByHourAndCityRequestDto requestDto)
        {
            try
            {
                var result = await _service.GetTotalByHourAndCityAsync(requestDto);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"The {nameof(GetTotalAmoutPaidByHourdAndCityAsync)} controller thrown an unnexpected error. Error: {ex.Message}. By the request object: {nameof(ReportTotalAmountPaidByHourAndCityRequestDto)} {JsonSerializer.Serialize(requestDto)}");
                throw new Exception($"Unnexpected error : {ex.Message}", ex);
            }
        }

        [HttpGet("report-total-vehicle-by-toll-station")]
        public async Task<IActionResult> GetVehicleCountByTollStationAsync([FromQuery] ReportVehicleCountByTollStationRequestDto requestDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(requestDto.TollStation))
                    return BadRequest("Toll station is required.");

                var result = await _service.GetVehicleTypeCountByStationAsync(requestDto.TollStation, requestDto.StartDate, requestDto.EndDate);

                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogError($"The {nameof(GetVehicleCountByTollStationAsync)} controller thrown an unnexpected error. Error: {ex.Message}. By the request object: {nameof(ReportVehicleCountByTollStationRequestDto)} {JsonSerializer.Serialize(requestDto)}");
                throw new Exception($"Unnexpected error : {ex.Message}", ex);
            }
        }

        [HttpGet("report-top-toll-station-highest-revenue")]
        public async Task<IActionResult> GetTopTollStationsWithHighestRevenueAsync([FromQuery] ReportTopTollStationsWithHighestRevenueByMonthRequestDto requestDto)
        {
            try
            {
                if (requestDto.NumberOfTollStations <= 0)
                    return BadRequest("Number of toll station must be major than 0.");

                if(requestDto.StartDate > requestDto.EndDate)
                    return BadRequest("The start date must be less than or equal the end date.");

                var result = await _service.GetTopTollStationsWithHighestRevenueAsync(requestDto);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"The {nameof(GetTopTollStationsWithHighestRevenueAsync)} controller thrown an unnexpected error. Error: {ex.Message}. By the request object: {nameof(ReportTopTollStationsWithHighestRevenueByMonthRequestDto)} {JsonSerializer.Serialize(requestDto)}");
                throw new Exception($"Unnexpected error : {ex.Message}", ex);
            }
        }
    }
}