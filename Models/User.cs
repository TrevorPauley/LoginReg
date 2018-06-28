using System.ComponentModel.DataAnnotations;
using LoginReg.Models;
using System.Collections.Generic;
using System;
using System.Security.Claims;

namespace LoginReg.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Cash { get; set; }
        public List<Auction> MyAuctions { get; set; }
        public List<Bid> MyBids { get; set; }
        public User()
        {
            MyAuctions = new List<Auction>();
            MyBids = new List<Bid>();
            this.Cash = 1000;
        }

    }
}