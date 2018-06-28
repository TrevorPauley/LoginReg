using System.ComponentModel.DataAnnotations;
using LoginReg.Models;
using System;
using System.Security.Claims;
using System.Collections.Generic;

namespace LoginReg.Models
{
    public class ValidAuction
    {
        public int AuctionID { get; set; }

        [Required]
        [MinLength(3)]
        public string Product { get; set; }

        public string Seller { get; set; }

        [Required]
        [Range(1, 1000000)]
        public int StartingBid { get; set; }

        [Required]
        [MinLength(10)]
        public string Description { get; set; }

        [Required]
        public DateTime TimeRemaining { get; set; }
    }
}
