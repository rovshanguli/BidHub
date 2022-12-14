using BidHub.Models;
using Microsoft.EntityFrameworkCore;

namespace BidHub.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public virtual DbSet<Bid>? Bids { get; set; }
    }
}
