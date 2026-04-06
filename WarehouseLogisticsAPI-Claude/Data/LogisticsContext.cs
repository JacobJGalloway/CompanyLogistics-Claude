using Microsoft.EntityFrameworkCore;

namespace WarehouseLogistics_Claude.Data
{
    public class LogisticsContext(DbContextOptions<LogisticsContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure entity relationships and keys here if needed
        }
    }
}