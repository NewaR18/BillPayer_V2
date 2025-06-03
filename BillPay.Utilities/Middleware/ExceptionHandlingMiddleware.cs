using Azure;
using BillPay.Models.Global;
using Hangfire.Logging;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BillPay.Utilities.Middleware
{
	public class ExceptionHandlingMiddleware : IMiddleware
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		public ExceptionHandlingMiddleware(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}
		public async Task InvokeAsync(HttpContext context, RequestDelegate next)
		{
			ClaimsPrincipal user = _httpContextAccessor.HttpContext?.User!;
			try
			{
				await next(context);
			}
			catch(Exception ex)
			{
				await WriteExceptionLog(context, ex);
				#region In Case of APIS
				/*DataResult response;
				response = new DataResult
				{
					Success = false,
					Error = ex.Message + " " + ex.InnerException?.ToString(),
					Status = context.Response.StatusCode,
					Message = ex.Message
				};
				await context.Response.WriteAsync(JsonSerializer.Serialize(response));*/
				#endregion
			}
		}
		private async Task WriteExceptionLog(HttpContext context, Exception ex)
		{
			string errorMessage = ex.Message + " " + ex.InnerException?.ToString();
			string controllerName = context.GetRouteValue("controller")?.ToString()!;
			string actionName = context.GetRouteValue("action")?.ToString()!;
			using (LogContext.PushProperty("Username", context.User.Identity!.IsAuthenticated ? context.User.Identity.Name : "Anonymous"))
			using (LogContext.PushProperty("ControllerName", controllerName))
			using (LogContext.PushProperty("ActionName", actionName))
			using (LogContext.PushProperty("InnerException", ex.InnerException))
			using (LogContext.PushProperty("StackTrace", ex.StackTrace))
			{
				Log.Error(ex, ex.Message);
			}
			string redirectUrl = $"/Bills/Home/ExceptionScreen?errormessage={errorMessage}";
			context.Response.Redirect(redirectUrl);
		}
	}
}
