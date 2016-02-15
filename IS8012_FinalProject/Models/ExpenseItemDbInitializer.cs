using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace IS8012_FinalProject.Models
{
    public class ExpenseItemDbInitializer : DropCreateDatabaseIfModelChanges<ExpenseItemDb>
    {
        protected override void Seed(ExpenseItemDb context)
        {
            base.Seed(context);
        }
    }
}