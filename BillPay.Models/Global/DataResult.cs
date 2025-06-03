using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BillPay.Models.Global
{
	public class DataResult<T> where T : class
	{
		public bool Success { get; set; }
		public int Status { get; set; }
		public string Message { get; set; }
		public T Data { get; set; }

		[JsonIgnore]
		public string Error { get; set; }

	}
	public class DataResult
	{
		[JsonPropertyName("success")]
		public bool Success { get; set; }
		[JsonPropertyName("status")]
		public int Status { get; set; }
		[JsonPropertyName("message")]
		public string Message { get; set; }
		[JsonPropertyName("error")]
		public string Error { get; set; }
		[JsonPropertyName("data")]
		public string Data { get; set; }
	}
}
