using IS8012_FinalProject.Models;
using IS8012_FinalProject.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace IS8012_FinalProject.Controllers
{
    public class ExpenseCategoryController : Controller
    {
        private ExpenseCategoryManager categoryManager;

        public ExpenseCategoryController()
        {
            this.categoryManager = new ExpenseCategoryManager();
        }
        // GET: ExpenseCategory
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetExpenseCategory(string userName)
        {
            if (ModelState.IsValid)
            {
                var results = categoryManager.GetCategoryByUserName(userName);
                return Json(results, JsonRequestBehavior.AllowGet);
            }
            else
                return new HttpStatusCodeResult(HttpStatusCode.NotFound); ;
        }

        [HttpPost]
        public ActionResult CreateExpenseCategory(ExpenseCategory item)
        {
            if (ModelState.IsValid)
            {
                this.categoryManager.AddCategory(item);
                return Content(JsonConvert.SerializeObject(item, Formatting.None, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd hh:mm:ss" }));
            }
            else
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [HttpPost]
        public ActionResult DeleteCategory(int id)
        {
            if (ModelState.IsValid)
            {
                this.categoryManager.DeleteCategory(id);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            else
                return new HttpStatusCodeResult(300);
        }

        public ActionResult EditCategory(int id, ExpenseCategory expenseItem)
        {
            if (ModelState.IsValid)
            {
                this.categoryManager.EditCategory(id, expenseItem);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            else
                return new HttpStatusCodeResult(300);
        }


    }
}