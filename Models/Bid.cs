using System.ComponentModel.DataAnnotations;
using LoginReg.Models;
using System;
using System.Security.Claims;

namespace LoginReg.Models
{
    public class Bid 
    {
        public int BidID { get; set; }
        public int UserID { get; set; }
        public int AuctionID { get; set; }
        public int BidAmount { get; set; }
        public User Users { get; set; }
        public Auction Auctions { get; set; }
    }
}