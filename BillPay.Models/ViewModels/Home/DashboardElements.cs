using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.Models.ViewModels.Home
{
    public class DashboardElements
    {
        public string Name { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Expenses { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalDue { get; set; }
        public string LastPaymentDate { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal ToReceive { get; set; }

    }
}
