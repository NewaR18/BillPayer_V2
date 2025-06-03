using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.Models.ViewModels.Home
{
    public class PaymentReport
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public bool Paid { get; set; }
        public string PayerUserId { get; set; }
        public int BillSummaryId { get; set; }
    }
}
