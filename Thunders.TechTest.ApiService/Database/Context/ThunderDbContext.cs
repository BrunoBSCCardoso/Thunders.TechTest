using Microsoft.EntityFrameworkCore;
using Thunders.TechTest.ApiService.DataBase.Models;

namespace Thunders.TechTest.ApiService.DataBase.Context
{
    public class ThunderDbContext : DbContext
    {
        public ThunderDbContext(DbContextOptions<ThunderDbContext> options) : base(options) { }

        public DbSet<TollStationUsage> TollStationUsages { get; set; }
        public DbSet<EventLog> EventLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TollStationUsage>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.StationName)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(p => p.City)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(p => p.State)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.AmountPaid)
                      .HasColumnType("decimal(10,2)");

                entity.Property(e => e.VehicleType)
                      .HasConversion<int>();
            });

            modelBuilder.Entity<EventLog>(entity =>
            {
                entity.HasKey(p => p.EventId);

                entity.Property(p => p.Status)
                      .IsRequired()
                      .HasMaxLength(50);
            });
        }

    }
}