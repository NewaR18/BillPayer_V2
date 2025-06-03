using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.Models.ViewModels.Bills
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string User { get; set; }
        public DateTime Date { get; set; }
        public string ProductName { get; set; }
        public int BillSummaryId { get; set; }
        public int BhukkadsId { get; set; }
        [Range(0.00,100.00, ErrorMessage ="Quantity cannot be negative")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Qty { get; set; }
        [Range(0.00,100000, ErrorMessage = "Price cannot be negative")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; }
        [Range(0.00, 100000, ErrorMessage = "Total cannot be negative")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }
    }
}
