using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq;
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

            var result = await query.GroupBy(t => new
                              {
                                  Hour = t.Timestamp,
                                  t.City
                              })
                              .Select(p => new ReportTotalAmountPaidByHourAndCityResponseDto
                              {
                                  TimeStamp = p.Key.Hour,
                                  City = p.Key.City,
                                  TotalAmount = p.Sum(x => x.AmountPaid)
                              })
                              .OrderBy(o => o.TimeStamp)
                              .ToListAsync();

            await _redisCache.GenerateCache(result, cacheKey);

            return result;
        }

        public async Task<IList<ReportVehicleTypeCountByStationResponseDto>> GetVehicleTypeCountByStationAsync(string tollStation, DateTime startDate, DateTime endDate)
        {
            var cacheKey = $"report:vehicle-count:{tollStation.ToLower()}{startDate}{endDate}";
            var cached = await _redisCache.GetCacheByKey(cacheKey);

            if (!string.IsNullOrEmpty(cached))
                return JsonSerializer.Deserialize<List<ReportVehicleTypeCountByStationResponseDto>>(cached)!;

            var query = _context.TollStationUsages
                .Where(t => t.StationName == tollStation
                            && (t.Timestamp >= startDate && t.Timestamp <= endDate))
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
                    TotalRevenue = g.Count()
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
