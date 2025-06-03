using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.Models.ViewModels.Bills
{
    public class PaymentDueAccounts
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalDue { get; set; }
    }
}
