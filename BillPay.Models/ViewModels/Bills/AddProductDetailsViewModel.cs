using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.Models.ViewModels.Bills
{
    public class AddProductDetailsViewModel
    {
        public string User { get; set; }
        public DateTime Date { get; set; }
        public int BillSummaryId { get; set; }
        public int BhukkadsId { get; set; }
        public ProductDetails ProductDetails { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> ProductList { get; set; }
    }
}
