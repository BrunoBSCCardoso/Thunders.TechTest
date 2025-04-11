using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Thunders.TechTest.ApiService.API.Dtos;
using Thunders.TechTest.ApiService.Events;
using Thunders.TechTest.ApiService.Interfaces;

namespace Thunders.TechTest.ApiService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TollStationRegisterController : ControllerBase
    {
        private readonly ITollStationUsageRegisteredProducer _producer;
        private readonly ILogger<TollStationRegisterController> _logger;

        public TollStationRegisterController(ITollStationUsageRegisteredProducer producer, ILogger<TollStationRegisterController> logger)
        {
            _producer = producer;
            _logger = logger;
        }


        [HttpPost]
        public async Task<IActionResult> CreateNewRegisterAsync([FromBody] TollStationOperationDto requestDto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var messageEvent = new TollStationUsageRegisteredEvent
                {
                    Timestamp = requestDto.Timestamp,
                    StationName = requestDto.StationName,
                    City = requestDto.City,
                    State = requestDto.State,
                    AmountPaid = requestDto.AmountPaid,
                    VehicleType = (int)requestDto.VehicleType
                };

                await _producer.RegisterAsync(messageEvent);

                _logger.LogDebug($"Event registered successfully with the EventID {messageEvent.EventId}");

                return Accepted();
            }
            catch (Exception ex)
            {
                _logger.LogError($"The {nameof(CreateNewRegisterAsync)} controller thrown an unnexpected error. Error: {ex.Message}. By the request object: {nameof(ReportTotalAmountPaidByHourAndCityRequestDto)} {JsonSerializer.Serialize(requestDto)}");
                throw new Exception("Unnexpected error : ", ex);
            }
        }
    }
}