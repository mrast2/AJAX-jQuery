using IS8012_FinalProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IS8012_FinalProject.Services
{
    public class ExpenseManager
    {
        #region variables
        //initializing database for Expense Items.
        private ExpenseItemDb db = new ExpenseItemDb();

        #endregion

        #region Methods

        /// <summary>
        /// Returns all the Expenses for a particular user
        /// </summary>
        /// <param name="userName">Username used to register in the client application</param>
        /// <returns></returns>
        internal ExpenseItem[] GetExpenseItems(string userName)
        {
            var result = db.ExpenseItems.Where(x => x.UserName == userName).OrderBy(x => x.ExpenseDate).ToArray();
            if (result == null)
            {
                throw new ArgumentNullException("item");
            }
            foreach (var item in result)
            {
                item.ExpenseDate = item.ExpenseDate.Date;
            }
            return result;
        }

        /// <summary>
        /// Gives a single expense item based on the item id.
        /// </summary>
        /// <param name="id">ID of the expense item</param>
        /// <returns>Expense Item</returns>
        internal ExpenseItem GetExpenseItemByID(int id)
        {
            var result = db.ExpenseItems.Find(id);
            if (result == null)
            {
                throw new ArgumentNullException("item");
            }
            result.ExpenseDate = result.ExpenseDate.Date;
            return result;
        }

        /// <summary>
        /// Method to save an Expense Item to the database.
        /// </summary>
        /// <param name="item">Expense Item to be added to the database for the user</param>
        /// <returns></returns>
        internal bool SaveItem(ExpenseItem item)
        {
            try
            {
                item.ExpenseDate = item.ExpenseDate.Date;
                db.ExpenseItems.Add(item);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Method to edit an expense Item
        /// </summary>
        /// <param name="id">ID of the item</param>
        /// <param name="item">Expense Item with changed parameters</param>
        /// <returns></returns>
        internal ExpenseItem EditItem(int id, ExpenseItem item)
        {
            var result = db.ExpenseItems.Find(id);
            if (result == null)
            {
                throw new ArgumentNullException("item");
            }

            db.Entry(item).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return item;
        }

        /// <summary>
        /// Delete Expense Item corresponding to the ID 
        /// </summary>
        /// <param name="id">Id if the expense item to be added</param>
        /// <returns></returns>
        internal bool DeleteItem(int id)
        {
            try
            {
                var item = db.ExpenseItems.Find(id);
                db.ExpenseItems.Remove(item);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Method to retrieve expense items for a user according to the specified category
        /// </summary>
        /// <param name="userName">Username of the user requesting</param>
        /// <param name="category">Category for which the Expense items are to be retrieved.</param>
        /// <returns></returns>
        internal ExpenseItem[] GetExpenseItemsByCategory(string userName, string category)
        {
            var result = db.ExpenseItems.Where(x => x.UserName == userName && x.ExpenseCategory == category).OrderBy(x => x.ExpenseDate).ToArray();
            if (result == null)
            {
                throw new ArgumentNullException("item");
            }
            foreach (var item in result)
            {
                item.ExpenseDate = item.ExpenseDate.Date;
            }
            return result;
        }

        /// <summary>
        /// Method to get expense items for a user between two dates.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns>An array of Expense Items</returns>
        internal ExpenseItem[] GetExpensesByDate(string userName, DateTime startDate, DateTime endDate)
        {
            var results = db.ExpenseItems.Where(x => x.UserName == userName && x.ExpenseDate >= startDate && x.ExpenseDate <= endDate).OrderBy(x => x.ExpenseDate).ToArray();
            if (results == null)
            {
                throw new ArgumentNullException("item");
            }
            foreach (var item in results)
            {
                item.ExpenseDate = item.ExpenseDate.Date;
            }
            return results;
        }

        #endregion
    }
}