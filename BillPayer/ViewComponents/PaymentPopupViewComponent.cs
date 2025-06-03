using BillPay.DataAccess.Repository.IRepository;
using BillPay.Models;
using BillPay.Models.ViewModels.Home;
using BillPay.Utilities.RolesAndMenus;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BillPayer.ViewComponents
{
    public class PaymentPopupViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _repo;
        private readonly UserManager<ApplicationUser> _userManager;
        public PaymentPopupViewComponent(IUnitOfWork repo, UserManager<ApplicationUser> userManager)
        {
            _repo = repo;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync(int bhukkadsId)
        {
            EsewaTransferViewModel esewaDetails = new EsewaTransferViewModel();
            var paymentDetails  = _repo.BhukkadsRepo.GetPaymentFormByBhukkadsId(bhukkadsId);
            return View(paymentDetails);
        }
    }
}
