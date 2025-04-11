using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> CreateNewRegisterAsync([FromBody] TollStationOperationDto inputDto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var messageEvent = new TollStationUsageRegisteredEvent
                {
                    Timestamp = inputDto.Timestamp,
                    StationName = inputDto.StationName,
                    City = inputDto.City,
                    State = inputDto.State,
                    AmountPaid = inputDto.AmountPaid,
                    VehicleType = (int)inputDto.VehicleType
                };

                await _producer.RegisterAsync(messageEvent);

                _logger.LogDebug($"Event registered successfully with the EventID {messageEvent.EventId}");

                return Accepted();
            }
            catch (Exception ex)
            {
                throw new Exception("Unnexpected error : ", ex);
            }
        }
    }
}