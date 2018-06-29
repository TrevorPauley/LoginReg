using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using LoginReg.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace LoginReg.Controllers
{
    public class HomeController : Controller
    {
        private LoginReg.Models.LRContext _context;

        public HomeController(LoginReg.Models.LRContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View("Index");
        }

        [HttpPost]
        [Route("RegisterUser")]
        public IActionResult RegisterUser(ValidUser MyUser)
        {
            if (ModelState.IsValid)
            {
                User ExistingUser = _context.users.SingleOrDefault(user => user.Username == MyUser.Username);
                if (ExistingUser != null)
                {
                    ViewBag.Message = "An account with this email already exists.";
                    return View("Index", MyUser);
                }
                PasswordHasher<ValidUser> Hasher = new PasswordHasher<ValidUser>();
                MyUser.Password = Hasher.HashPassword(MyUser, MyUser.Password);
                User NewPerson = new User
                {
                    FirstName = MyUser.FirstName,
                    LastName = MyUser.LastName,
                    Username = MyUser.Username,
                    Password = MyUser.Password
                };
                ViewData.Clear();
                HttpContext.Session.Clear();
                _context.Add(NewPerson);
                // OR _context.users.Add(NewPerson);
                _context.SaveChanges();
                NewPerson = _context.users.SingleOrDefault(user => user.Username == NewPerson.Username);
                HttpContext.Session.SetString("Name", NewPerson.FirstName);
                var RegistrarID = _context.users.Where(Registrar => Registrar.UserID == NewPerson.UserID).First();
                HttpContext.Session.SetInt32("UserID", RegistrarID.UserID);
                TempData["UserID"] = HttpContext.Session.GetInt32("UserID");
                TempData["Name"] = HttpContext.Session.GetString("Name");
                return RedirectToAction("Success"); 
            }
            else
            {
                return View("Index", MyUser);
            }
        }

        [HttpPost]
        [Route("LoginUser")]
        public IActionResult LoginUser(string Username, string Password)
        {
            //Is user in DB? User where or SingleOrDefault or firstordefault
            User user = _context.users.Where(Loginer => Loginer.Username == Username).SingleOrDefault();
            if (user != null && Password != null)
            {
                var Hasher = new PasswordHasher<User>();
                // Pass the user object, the hashed password, and the PasswordToCheck
                if (0 != Hasher.VerifyHashedPassword(user, user.Password, Password))
                {
                    HttpContext.Session.Clear();
                    ViewData.Clear();
                    HttpContext.Session.SetString("Name", user.FirstName);
                    var LoginID = _context.users.Where(Loginer => Loginer.UserID == user.UserID).First();
                    HttpContext.Session.SetInt32("UserID", LoginID.UserID);
                    TempData["UserID"] = HttpContext.Session.GetInt32("UserID");
                    TempData["Name"] = HttpContext.Session.GetString("Name");
                    return RedirectToAction("Success");
                }
            }
            else
            {
                return View("Index", ViewBag.Message);
            }
            ViewBag.Message = "There was an issue with logging in, please try again.";
            return View("Index");
        }

        [HttpGet]
        [Route("Logout")]
        public IActionResult Logout()
        {
            ViewData.Clear();
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("Success")]
        public IActionResult Success()
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return View("Index");
            }
            else
            {
                ViewBag.auctions = _context.auctions.FromSql("SELECT * FROM auctiondb.auctions").ToList();
                List<Auction> Allauctions = _context.auctions.Include(b => b.Bids).ToList();
                ViewBag.TopBid = _context.bids.FromSql("SELECT * FROM auctiondb.bids").OrderBy(b => b.BidAmount).FirstOrDefault();
                ViewBag.Allauctions = Allauctions.OrderBy(t => t.TimeRemaining);
                ViewBag.UserID = TempData["UserID"];
                ViewBag.Name = TempData["Name"];
                ViewBag.Message = TempData["Message"];
                return View("Success");
            }
        }

        [HttpGet]
        [Route("Product/{AuctionID}")]
        public IActionResult AuctionPage(int AuctionID)
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return View("Index");
            }
            else
            {
                ViewBag.CurrentProduct = _context.auctions.Where(Product => Product.AuctionID == AuctionID).Include(u => u.Users).SingleOrDefault();
                //ViewBag.Bidding = _context.bids.Where(Bid => Bid.AuctionID == AuctionID).Include(u => u.users).OrderByDescending(b => b.BidAmount).FirstOrDefault();
                TempData["UserID"] = HttpContext.Session.GetInt32("UserID");
                TempData["Name"] = HttpContext.Session.GetString("Name");
                ViewBag.UserID = TempData["UserID"];
                ViewBag.Name = TempData["Name"];
                return View("Auction");
            }
        }

        [HttpPost]
        [Route("Bid/{AuctionID}")]
        public IActionResult Bid(int Amount, int AuctionID)
        {
            User Bidder = _context.users.SingleOrDefault(user => user.UserID == HttpContext.Session.GetInt32("UserID"));
            var Balance = Bidder.Cash;
            if(Bidder.Cash - Amount <= 0)
            {
                TempData["Message"] = "You do not have enough money to make that bid.";
                TempData["UserID"] = HttpContext.Session.GetInt32("UserID");
                TempData["Name"] = HttpContext.Session.GetString("Name");
                return RedirectToAction("Success", AuctionID);
            }
            else
            {
                var newBalance = Bidder.Cash - Amount;
                var Updated = Bidder;
                {
                    Bidder.FirstName = Bidder.FirstName;
                    Bidder.LastName = Bidder.LastName;
                    Bidder.Username = Bidder.Username;
                    Bidder.Password = Bidder.Password;
                    Bidder.Cash = newBalance;
                };
                Models.Bid newBid = new Models.Bid
                {
                    UserID = Bidder.UserID,
                    AuctionID = AuctionID,
                    BidAmount = Amount,
                };
                _context.Update(Updated);
                _context.Update(newBid);
                _context.SaveChanges();
                return RedirectToAction("Success");
            }
        }

        [HttpGet]
        [Route("New")]
        public IActionResult New()
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return View("Index");
            }
            else
            {
                TempData["UserID"] = HttpContext.Session.GetInt32("UserID");
                TempData["Name"] = HttpContext.Session.GetString("Name");
                ViewBag.Name = TempData["Name"];
                ViewBag.UserID = TempData["UserID"];
                return View("New");
            }
        }

        [HttpPost]
        [Route("NewAuction")]
        public IActionResult NewAuction(ValidAuction SubAuction)
        {
            Models.Auction CreateAuction = new Models.Auction
            {
                Product = SubAuction.Product,
                Seller = HttpContext.Session.GetString("Name"),
                UserID = (int)HttpContext.Session.GetInt32("UserID"),
                StartingBid = SubAuction.StartingBid,
                TimeRemaining = SubAuction.TimeRemaining,
                Description = SubAuction.Description

            };
            TempData["UserID"] = HttpContext.Session.GetInt32("UserID");
            TempData["Name"] = HttpContext.Session.GetString("Name");
            _context.Add(CreateAuction);
            _context.SaveChanges();
            return RedirectToAction("Success");
        }
        
        [HttpGet]
        [Route("Delete/{AuctionID}")]
        public IActionResult DeleteAuction(int AuctionID)
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return View("Index");
            }
            else
            {
                var myAuction = _context.auctions.SingleOrDefault(A => A.AuctionID == AuctionID);
                TempData["UserID"] = HttpContext.Session.GetInt32("UserID");
                TempData["Name"] = HttpContext.Session.GetString("Name");
                _context.auctions.Remove(myAuction);
                _context.SaveChanges();
                return RedirectToAction("Success");
            }
        }
    }
}
