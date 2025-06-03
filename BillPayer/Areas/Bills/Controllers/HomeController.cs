using BillPay.DataAccess.Repository.IRepository;
using BillPay.Models;
using BillPay.Models.ViewModels.Common;
using BillPay.Models.ViewModels.Home;
using BillPay.Utilities.BackgroundJobs.RecurringJobs;
using BillPayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;
using System.Diagnostics;
using System.Reflection;
using System.Security.Claims;
using static System.Formats.Asn1.AsnWriter;

namespace BillPayer.Areas.Bills.Controllers
{
    [Area("Bills")]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _repo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmailJob _emailJob;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork repo, UserManager<ApplicationUser> userManager, EmailJob emailJob)
        {
            _logger = logger;
            _repo = repo;
            _userManager = userManager;
            _emailJob = emailJob;
        }

        public IActionResult Index()
        {
            //_emailJob.SendEmailDaily();
            DashboardElements dashboardElements = new DashboardElements();
            var user = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            try
            {
                if (user != null)
                {
                    dashboardElements = _repo.HomeRepo.GetDashboardElements(user.Id);
                    dashboardElements.Name = user.Name;
                }
            }
            catch
            {
                throw;
            }
            return View(dashboardElements);
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
		public IActionResult ExceptionScreen(string errormessage)
		{
			return View("ExceptionScreen",errormessage);
		}
		public IActionResult PaymentReport()
        {
            return View();
        }
        public IActionResult BhukkadDetails(int bhukkadId)
        {
            return View();
        }

        #region APIS

        [HttpGet]
        public JsonResult getLineTrend()
        {
            var user = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            try
            {
                if (user != null)
                {
                    IEnumerable<LineChart> bll = _repo.HomeRepo.GetLineTrend(user.Id);
                    if (bll != null)
                    {
                        return Json(new { success = true, data = bll });
                    }
                }
            }
            catch
            {
                throw;
            }
            return Json(new { success = false });
        }
        [HttpGet]
        public JsonResult PayDetails(int bhukkadsId)
        {
            var user = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            try
            {
                PaymentDetails paymentDetails = _repo.BhukkadsRepo.GetPaymentFormByBhukkadsId(bhukkadsId);
                if (paymentDetails == null)
                {
                    string errorMessage = "Error Fetching Data | PayDetails";
                    return Json(new { success = false, message = errorMessage });
                }
                if ((paymentDetails.EsewaName == null || paymentDetails.EsewaPhone == null) && paymentDetails.Name != null)
                {
                    string errorMessage = "User " + paymentDetails.Name + " has not updated the esewa details in profile section";
                    return Json(new { success = false, message = errorMessage });
                }
                return Json(new { success = true, data = paymentDetails });
            }
            catch
            {
                throw;
            }
            
        }
        [HttpGet]
        public JsonResult ReceivableDetails(int billSummaryId,bool paid)
        {
            var user = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            try
            {
                ReportDetails receivableDetails = _repo.BillSummaryRepo.GetPaymentFormByBillSummaryId(billSummaryId, paid);
                if (receivableDetails == null)
                {
                    string errorMessage = "Error Fetching Data | ReceivableDetails";
                    return Json(new { success = false, message = errorMessage });
                }
                return Json(new { success = true, data = receivableDetails });
            }
            catch
            {
                throw;
            }
        }
        [HttpGet]
        public JsonResult PaidByUser(int bhukkadsId)
        {
            var user = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            try
            {
                bool paymentDetails = _repo.BhukkadsRepo.UpdatePaymentStatus(bhukkadsId);
                if (paymentDetails == false)
                {
                    string errorMessage = "Error Updating Payment Status";
                    return Json(new { success = false, message = errorMessage, paid = false });
                }
                DashboardElements dashboardElements = GetDashboardElements();
                if (dashboardElements == null)
                {
                    string errorMessage = "Could not refresh dashboard data";
                    return Json(new { success = false, message = errorMessage, paid = true });
                }
                string successMessage = "Paid status updated successfully";
                return Json(new { success = true, message = successMessage, data = dashboardElements });
            }
            catch
            {
                throw;
            }
        }
        [HttpGet]
        public JsonResult GetBhukkadList()
        {
            var user = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            try
            {
                if (user != null)
                {
                    IEnumerable<UnpaidBhukkads> bll = _repo.BhukkadsRepo.GetUnpaidBhukkadListForDashboard(user.Id);
                    if (bll != null)
                    {
                        return Json(new { success = true, data = bll });
                    }
                }
                return Json(new { success = false });
            }
            catch
            {
                throw;
            }
        }
        public DashboardElements GetDashboardElements()
        {
            try
            {
                var user = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
                DashboardElements dashboardElements = new DashboardElements();

                if (user != null)
                {
                    dashboardElements = _repo.HomeRepo.GetDashboardElements(user.Id);
                    if (dashboardElements != null)
                    {
                        return dashboardElements;
                    }
                }
                return null!;
            }
            catch 
            {
                throw;
            }
        }
        [HttpPost]
        public JsonResult GetAll([FromBody] PaginationViewModel model)
        {
            try
            {
                var TotalRecord = 0;
                if (User.IsInRole("Admin") || User.IsInRole("Super Admin"))
                {
                    TotalRecord = _repo.BhukkadsRepo.GetCount();
                }
                else
                {
                    var userClaimsIdentity = (ClaimsIdentity)User.Identity!;
                    var userIdClaim = userClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                    if (string.Equals(model.Status == null ? "" : model.Status, "toReceive", StringComparison.OrdinalIgnoreCase))
                    {
                        TotalRecord = _repo.BhukkadsRepo.GetToReceiveCount(userIdClaim.Value);
                    }
                    else if (string.Equals(model.Status == null ? "" : model.Status, "received", StringComparison.OrdinalIgnoreCase))
                    {
                        TotalRecord = _repo.BhukkadsRepo.GetReceivedCount(userIdClaim.Value);
                    }
                    else
                    {
                        TotalRecord = _repo.BhukkadsRepo.GetCountOfUser(userIdClaim.Value);
                    }
                }
                PaginatedPaymentReport entities = _repo.BhukkadsRepo.GetPaginatedRows(model, User);
                var responseData = new
                {
                    draw = model.Draw,
                    recordsTotal = TotalRecord,
                    recordsFiltered = entities.TotalCount,
                    data = entities.Data
                };
                return Json(responseData);
            }
            catch
            {
                throw;
            }
            
        }
        #endregion
    }
}