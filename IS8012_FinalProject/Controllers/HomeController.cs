using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IS8012_FinalProject.Controllers
{
    public class HomeController : Controller
    {
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

        [Authorize(Roles = "")]
        public ActionResult ExpenseManager()
        {
            ViewBag.Title = "Expense Manager";
            ViewBag.UserName = User.Identity.GetUserName();
            return View();
        }

        [Authorize(Roles = "")]
        public ActionResult ManageCategory()
        {
            ViewBag.Title = "Manage Expense Category";
            ViewBag.UserName = User.Identity.GetUserName();
            return View();
        }
    }
}