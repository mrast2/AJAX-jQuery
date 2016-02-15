using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace IS8012_FinalProject.Models
{
    public class ExpenseItemDb : DbContext
    {
        public DbSet<ExpenseItem> ExpenseItems { get; set; }
    }
}