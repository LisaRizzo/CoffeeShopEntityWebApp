using CoffeeShopEntityDatabase.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CoffeeShopEntityDatabase.Controllers
{
    public class HomeController : Controller
    {
        ShopDBEntities db = new ShopDBEntities();
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
            User user = db.Users.Where(usr => usr.UserName == u.UserName && usr.Password == u.Password).ToList().First();
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
                User usr = db.Users.SingleOrDefault(u => u.Id == user.Id);
                Item item = db.Items.SingleOrDefault(x => x.Id == i.Id);
                decimal difference = decimal.Parse(usr.Funds.ToString()) - cost;
                usr.Funds = difference;
                item.Qty -= 1;
                db.Users.AddOrUpdate(usr);
                db.Items.AddOrUpdate(item);
                //User item object then set the user ID prop to the ID of the user that bought the item. 
                //Also set the itemid prop of the item that was bought. 
                //Then add or update the user items table and put in the user item obj
                UserItem uitem = db.UserItems.SingleOrDefault(x => x.ItemID == i.Id);
                UserItem uuser = db.UserItems.SingleOrDefault(x => x.UserID == usr.Id);
                db.UserItems.AddOrUpdate(uitem, uuser);
                db.SaveChanges();
                Session["User"] = usr;
            }
            else
            {
                Session["UserFunds"] = user.Funds;
                Session["ItemCost"] = i.Price;
                return RedirectToAction("ErrorPage");
            }
            return RedirectToAction("Shop");
        }

        public ActionResult ErrorPage()
        {
            return View();
        }

        public ActionResult Logout()
        {
            Session["User"] = null;
            return RedirectToAction("Login");
        }
    }
}