using Microsoft.EntityFrameworkCore;

namespace RailwayApi.Models
{
    public class RailwayContext : DbContext {
        public RailwayContext(DbContextOptions< RailwayContext > options)
            : base(options)
        {
        }

        public DbSet< Ticket > tickets { get; set; }
    }
}