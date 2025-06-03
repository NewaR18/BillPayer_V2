using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.Models
{
    public class Bhukkads
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalOfPerson { get; set; }
        public bool Paid { get; set; }
        [Required]
        public int BillSummaryId { get; set; }
        [ForeignKey("BillSummaryId")]
        [ValidateNever]
        public BillSummary BillSummary { get; set; }
        public virtual List<ProductDetails> Products { get; set; } = new List<ProductDetails>();
    }
}
