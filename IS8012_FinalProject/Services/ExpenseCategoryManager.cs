using IS8012_FinalProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IS8012_FinalProject.Services
{
    public class ExpenseCategoryManager
    {
        private ExpenseCategoryDb db = new ExpenseCategoryDb();

        #region Methods for Expense Category Controller

        internal ExpenseCategory[] GetCategoryByUserName(string userName)
        {
            var result = db.ExpenseCategories.Where(x => x.UserName == userName).OrderBy(x => x.CategoryName).ToArray();
            return result;
        }

        internal bool AddCategory(ExpenseCategory category)
        {
            try
            {
                db.ExpenseCategories.Add(category);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        internal object EditCategory(int id, ExpenseCategory expenseItem)
        {
            var result = db.ExpenseCategories.Find(id);
            if (result == null)
            {
                throw new ArgumentNullException("item");
            }
            result.CategoryName = expenseItem.CategoryName;
            db.Entry(result).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return result;
        }

        internal bool DeleteCategory(int id)
        {
            try
            {
                var item = db.ExpenseCategories.Find(id);
                db.ExpenseCategories.Remove(item);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
    }
}