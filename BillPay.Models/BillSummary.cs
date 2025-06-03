using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.Models
{
    public class BillSummary
    {
        public int Id { get; set; }
        [DisplayName("Paid By")]
        public string PayerUserId { get; set; }
        public DateTime Date { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal GrandTotal { get; set; }
        public virtual List<Bhukkads> BhukkadsList { get; set; } = new List<Bhukkads>();
    }
}
