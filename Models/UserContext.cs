using Microsoft.EntityFrameworkCore;
using LoginReg.Models;

namespace LoginReg.Models
{
    public class LRContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public LRContext(DbContextOptions<LRContext> options) : base(options) { }

        public DbSet<LoginReg.Models.User> Users { get; set; }
        public DbSet<LoginReg.Models.Auction> Auctions { get; set; }
        public DbSet<LoginReg.Models.Bid> Bids { get; set; }
    }
}
