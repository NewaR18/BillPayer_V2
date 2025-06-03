using BillPay.Models.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.Models.ViewModels.Common
{
    public class PaginatedPaymentReport
    {
        public IEnumerable<PaymentReport> Data { get; set; }
        public int TotalCount { get; set; }
    }
}
