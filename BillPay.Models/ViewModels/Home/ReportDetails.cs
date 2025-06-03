using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.Models.ViewModels.Home
{
    public class ReportDetails
    {
        public string Payer { get; set; }
        public DateTime Date { get; set; }
        public string GrandTotal { get; set; }
        public List<BhukkadTotal>  BhukkadsTotal { get; set; }
    }
    public class BhukkadTotal
    {
        public string Bhukkad { get; set; }
        public decimal Total { get; set; }
    }
}
