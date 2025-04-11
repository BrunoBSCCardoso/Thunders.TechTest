using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Thunders.TechTest.ApiService.API.Dtos;
using Thunders.TechTest.ApiService.DataBase.Context;
using Thunders.TechTest.ApiService.Interfaces;

namespace Thunders.TechTest.ApiService.Services
{
    public class ReportService : IReportService
    {
        private readonly ThunderDbContext _context;
        private readonly IRedisService _redisCache;

        public ReportService(ThunderDbContext context, IRedisService redisCache)
        {
            _context = context;
            _redisCache = redisCache;
        }

        public async Task<IList<ReportTotalAmountPaidByHourAndCityResponseDto>> GetTotalByHourAndCityAsync(ReportTotalAmountPaidByHourAndCityRequestDto requestDto)
        {
            string cacheKey = $"report:hourly-city-total-amout-{requestDto.City.ToLower()}";
            var cached = await _redisCache.GetCacheByKey(cacheKey);

            if (!string.IsNullOrEmpty(cached))
                return JsonSerializer.Deserialize<List<ReportTotalAmountPaidByHourAndCityResponseDto>>(cached);

            var query = _context.TollStationUsages.AsQueryable();

            if (!string.IsNullOrEmpty(requestDto.City))
            {
                query = query.Where(w => w.City == requestDto.City);
            }

            var rawData = await query.GroupBy(t => new
                                  {
                                      t.Timestamp.Year,
                                      t.Timestamp.Month,
                                      t.Timestamp.Day,
                                      t.Timestamp.Hour,
                                      t.City
                                  })
                                  .Select(g => new
                                  {
                                      g.Key.Year,
                                      g.Key.Month,
                                      g.Key.Day,
                                      g.Key.Hour,
                                      g.Key.City,
                                      Total = g.Sum(x => x.AmountPaid)
                                  })
                                  .ToListAsync();

            var result = rawData.Select(r => new ReportTotalAmountPaidByHourAndCityResponseDto
                                {
                                    TimeStamp = new DateTime(r.Year, r.Month, r.Day, r.Hour, 0, 0),
                                    City = r.City,
                                    TotalAmount = r.Total
                                })
                                .OrderBy(r => r.TimeStamp)
                                .ToList();

            await _redisCache.GenerateCache(result, cacheKey);

            return result;
        }

        public async Task<IList<ReportVehicleTypeCountByStationResponseDto>> GetVehicleTypeCountByStationAsync(ReportVehicleCountByTollStationRequestDto requestDto)
        {
            var cacheKey = $"report:vehicle-count:{requestDto.TollStation.ToLower()}{requestDto.StartDate}{requestDto.EndDate}";
            var cached = await _redisCache.GetCacheByKey(cacheKey);

            if (!string.IsNullOrEmpty(cached))
                return JsonSerializer.Deserialize<List<ReportVehicleTypeCountByStationResponseDto>>(cached)!;

            var query = _context.TollStationUsages
                .Where(t => t.StationName == requestDto.TollStation
                            && (t.Timestamp >= requestDto.StartDate && t.Timestamp <= requestDto.EndDate))
                .GroupBy(t => t.VehicleType)
                .Select(g => new ReportVehicleTypeCountByStationResponseDto
                {
                    VehicleType = g.Key,
                    Amount = g.Count()
                })
                .AsQueryable();

            var result = await query.ToListAsync();

            await _redisCache.GenerateCache(result, cacheKey);

            return result;
        }

        public async Task<IList<ReportTopTollStationsWithHighestRevenueByMonthResponseDto>> GetTopTollStationsWithHighestRevenueAsync(ReportTopTollStationsWithHighestRevenueByMonthRequestDto requestDto)
        {
            var cacheKey = $"report:top-toll-station-highest-revenue:{requestDto.NumberOfTollStations}{requestDto.StartDate}{requestDto.EndDate}";
            var cached = await _redisCache.GetCacheByKey(cacheKey);

            if (!string.IsNullOrEmpty(cached))
                return JsonSerializer.Deserialize<List<ReportTopTollStationsWithHighestRevenueByMonthResponseDto>>(cached)!;

            var query = _context.TollStationUsages
                .Where(t => (t.Timestamp >= requestDto.StartDate && t.Timestamp <= requestDto.EndDate))
                .GroupBy(t => t.StationName)
                .Select(g => new ReportTopTollStationsWithHighestRevenueByMonthResponseDto
                {
                    TollStation = g.Key,
                    TotalRevenue = g.Sum(x => x.AmountPaid)
                })
                .OrderByDescending(o => o.TotalRevenue)
                .Take(requestDto.NumberOfTollStations)
                .AsQueryable();

            var result = await query.ToListAsync();

            await _redisCache.GenerateCache(result, cacheKey);

            return result;
        }


    }
}
