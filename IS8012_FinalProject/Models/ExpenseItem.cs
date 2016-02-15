using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IS8012_FinalProject.Models
{
    public class ExpenseItem
    {
        [Key]
        public int ItemID { get; set; }

        [Required]
        public string ExpenseName { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string ExpenseCategory { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [DataType(DataType.Date)]
        public DateTime ExpenseDate { get; set; }

    }
}