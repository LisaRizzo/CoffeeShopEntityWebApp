using CoffeeShopEntityDatabase.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CoffeeShopEntityDatabase.Controllers
{
    public class HomeController : Controller
    {
        ShopDBEntities db = new ShopDBEntities ();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        public ActionResult Register()
        {

            return View();
        }

        [HttpPost]
        public ActionResult MakeNewUser(User u)
        {
            db.Users.Add(u);
            db.SaveChanges();

            return RedirectToAction("Login");
        }

        public ActionResult Login()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Login(User u)
        {
                User user  = db.Users.Where(usr => usr.UserName == u.UserName && usr.Password == u.Password).ToList().First();
                 Session["User"] = user;
                 return RedirectToAction("Shop");
        }
        public ActionResult Shop()
        {
            List<Item> itemlist = db.Items.ToList();
            return View(itemlist);
        }
        [HttpPost]
        public ActionResult BuyItem(Item i)
        {
            decimal cost = decimal.Parse(i.Price.ToString());
            User user = (User)Session["User"];
            
            if (user.Funds >= cost)
            {
                User u = db.Users.Find(user.Id);
                decimal d = decimal.Parse(u.Funds.ToString()) - cost;
                u.Funds = d;
                db.Users.AddOrUpdate(u);
                db.SaveChanges();
                Session["User"] = u;
            }
            else
            {
                return RedirectToAction("ErrorPage");
            }
            return RedirectToAction("Shop");
        }
    }
}