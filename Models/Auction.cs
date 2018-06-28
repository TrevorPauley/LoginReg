using System.ComponentModel.DataAnnotations;
using LoginReg.Models;
using System;
using System.Security.Claims;
using System.Collections.Generic;

namespace LoginReg.Models
{
    public class Auction
    {
        public int AuctionID { get; set; }
        public string Product { get; set; }
        public string Seller { get; set; }
        public int StartingBid { get; set; }
        public string Description { get; set; }
        public int UserID { get; set; }
        public DateTime TimeRemaining { get; set; }
        public User Users { get; set; }
        public List<Bid> Bids { get; set; }
        public Auction()
        {
            Bids = new List<Bid>();
        }
    }
}
