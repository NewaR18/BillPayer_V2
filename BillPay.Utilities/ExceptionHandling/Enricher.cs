using BillPay.Models.Global;
using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.Utilities.ExceptionHandling
{
	public static class Enricher
	{
		public static void HttpRequestEnricher(IDiagnosticContext diagnosticContext, HttpContext httpContext)
		{
			var httpContextInfo = new HttpContextInfo
			{
				Protocol = httpContext.Request.Protocol,
				Scheme = httpContext.Request.Scheme,
				IpAddress = httpContext.Connection.RemoteIpAddress.ToString(),
				Host = httpContext.Request.Host.ToString(),
				User = GetUserInfo(httpContext.User)
			};
            diagnosticContext.Set("HttpContext", httpContextInfo, true);
		}

		private static string GetUserInfo(ClaimsPrincipal user)
		{
			if (user.Identity != null && user.Identity.IsAuthenticated)
			{
				return user.Identity.Name;
			}
			return Environment.UserName;
		}
	}
}
