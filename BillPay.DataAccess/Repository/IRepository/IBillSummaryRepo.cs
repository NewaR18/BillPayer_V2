using BillPay.Models;
using BillPay.Models.ViewModels.Bills;
using BillPay.Models.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.DataAccess.Repository.IRepository
{
    public interface IBillSummaryRepo : IRepo<BillSummary>
    {
        public void Update(BillSummary entity);
        public new BillSummary GetFirstOrDefault(Expression<Func<BillSummary, bool>> filter, string? IncludeProperties = null);
        public bool UpdatePayerId(int BillSummaryId, string PayerId);
        public bool UpdateDate(int BillSummaryId, DateTime DateEaten);
        public bool UpdateGrandTotal(int BillSummaryId);
        public ProductViewModel GetInfoBasedOnProductDetails(int id);
        public ReportDetails GetPaymentFormByBillSummaryId(int billSummaryId, bool paid);
    }
}
