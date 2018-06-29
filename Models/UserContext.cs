using Microsoft.EntityFrameworkCore;
using LoginReg.Models;

namespace LoginReg.Models
{
    public class LRContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public LRContext(DbContextOptions<LRContext> options) : base(options) { }

        public DbSet<LoginReg.Models.User> users { get; set; }
        public DbSet<LoginReg.Models.Auction> auctions { get; set; }
        public DbSet<LoginReg.Models.Bid> bids { get; set; }
    }
}
