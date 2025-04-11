using Microsoft.EntityFrameworkCore;
using Moq;
using Thunders.TechTest.ApiService.API.Dtos;
using Thunders.TechTest.ApiService.DataBase.Context;
using Thunders.TechTest.ApiService.DataBase.Models;
using Thunders.TechTest.ApiService.Interfaces;
using Thunders.TechTest.ApiService.Services;
using Xunit;

namespace Thunders.TechTest.Tests
{
    public class ReportServiceTests
    {
        [Fact]
        public async Task GetTotalByHourAndCityAsync_ReturnsExpectedResult_WhenCacheIsEmpty()
        {
            // Arrange
            var city = "São Paulo";
            var dto = new ReportTotalAmountPaidByHourAndCityRequestDto { City = city };

            var options = new DbContextOptionsBuilder<ThunderDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            using var context = new ThunderDbContext(options);
            context.TollStationUsages.Add(new TollStationUsage
            {
                City = city,
                State = "SP",
                StationName = "Anhanguera",
                VehicleType = VehicleType.Car,
                AmountPaid = 10,
                Timestamp = DateTime.Now
            });
            await context.SaveChangesAsync();

            var redisMock = new Mock<IRedisService>();
            redisMock.Setup(r => r.GetCacheByKey(It.IsAny<string>())).ReturnsAsync((string)null);
            redisMock.Setup(r => r.GenerateCache(It.IsAny<object>(), It.IsAny<string>()));

            var service = new ReportService(context, redisMock.Object);

            // Act
            var result = await service.GetTotalByHourAndCityAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(city, result[0].City);
        }

        [Fact]
        public async Task GetVehicleCountByTollStationAsync_ReturnsVehicleTypesGroupedByStation()
        {
            var fixedDate = new DateTime(2024, 4, 10);

            var dto = new ReportVehicleCountByTollStationRequestDto 
            { 
                StartDate = fixedDate, 
                EndDate = fixedDate.AddDays(1),
                TollStation = "Anhanguera" 
            };

            var options = new DbContextOptionsBuilder<ThunderDbContext>()
                .UseInMemoryDatabase("TestDb2")
                .Options;

            using var context = new ThunderDbContext(options);
            context.TollStationUsages.AddRange(
                new TollStationUsage { StationName = "Anhanguera", VehicleType = VehicleType.Car, Timestamp = fixedDate },
                new TollStationUsage { StationName = "Anhanguera", VehicleType = VehicleType.Motocycle, Timestamp = fixedDate },
                new TollStationUsage { StationName = "Anhanguera", VehicleType = VehicleType.Car, Timestamp = fixedDate }
            );
            await context.SaveChangesAsync();

            var redisMock = new Mock<IRedisService>();
            redisMock.Setup(r => r.GetCacheByKey(It.IsAny<string>())).ReturnsAsync((string)null);
            redisMock.Setup(r => r.GenerateCache(It.IsAny<string>(), It.IsAny<string>()));

            var service = new ReportService(context, redisMock.Object);
            var result = await service.GetVehicleTypeCountByStationAsync(dto);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetTopTollStationsWithHighestRevenueAsync_ReturnsTopStations()
        {
            var fixedDate = new DateTime(2024, 4, 10);

            var dto = new ReportTopTollStationsWithHighestRevenueByMonthRequestDto 
            { 
                StartDate = fixedDate, 
                EndDate = fixedDate.AddDays(1), 
                NumberOfTollStations = 2 
            };

            var options = new DbContextOptionsBuilder<ThunderDbContext>()
                .UseInMemoryDatabase("TestDb3")
                .Options;

            using var context = new ThunderDbContext(options);
            context.TollStationUsages.AddRange(
                new TollStationUsage { StationName = "Anhanguera", AmountPaid = 50, Timestamp = fixedDate },
                new TollStationUsage { StationName = "Imigrantes", AmountPaid = 100, Timestamp = fixedDate },
                new TollStationUsage { StationName = "Anhanguera", AmountPaid = 75, Timestamp = fixedDate }
            );
            await context.SaveChangesAsync();

            var redisMock = new Mock<IRedisService>();
            redisMock.Setup(r => r.GetCacheByKey(It.IsAny<string>())).ReturnsAsync((string)null);
            redisMock.Setup(r => r.GenerateCache(It.IsAny<string>(), It.IsAny<string>()));

            var service = new ReportService(context, redisMock.Object);
            var result = await service.GetTopTollStationsWithHighestRevenueAsync(dto);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.TollStation == "Imigrantes");
            Assert.Contains(result, r => r.TollStation == "Anhanguera");
        }
    }
}
