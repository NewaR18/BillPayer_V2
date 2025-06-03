using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.Models.Global
{
	public class HttpContextInfo
	{
		public string IpAddress { get; set; }
		public string Host { get; set; }
		public string Protocol { get; set; }
		public string Scheme { get; set; }
		public string User { get; set; }
	}
}
