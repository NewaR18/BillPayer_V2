using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.Models
{
    public class ProcedureSeedingLog
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(256)]
        public string ProcedureName { get; set; }
        [Required]
        [StringLength(256)]
        public string ScriptHash { get; set; }
        [Required]
        public DateTime ExecutedDatetime { get; set; }
        [Required]
        public bool HasError { get; set; }
        [StringLength(500)]
        public string ErrorMessage { get; set; }
    }
}
