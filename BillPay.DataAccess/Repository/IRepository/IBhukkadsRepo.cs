using BillPay.Models;
using BillPay.Models.ViewModels.Bills;
using BillPay.Models.ViewModels.Common;
using BillPay.Models.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.DataAccess.Repository.IRepository
{
    public interface IBhukkadsRepo : IRepo<Bhukkads>
    {
        public void Update(Bhukkads entity);
        public AddProductDetailsViewModel GetInfoBasedOnBhukkadsId(int bhukkadsId);
        public IEnumerable<UnpaidBhukkads> GetUnpaidBhukkadListForDashboard(string userId);
        public PaymentDetails GetPaymentFormByBhukkadsId(int bhukkadsId);
        public bool UpdatePaymentStatus(int bhukkadsId);
        public IEnumerable<PaymentDueAccounts> GetDueAccounts();
        public int GetCountOfUser(string userId);
        public int GetToReceiveCount(string userId);
        public int GetReceivedCount(string userId);
        public PaginatedPaymentReport GetPaginatedRows(PaginationViewModel dataTableAjaxModel, ClaimsPrincipal User);
    }
}
