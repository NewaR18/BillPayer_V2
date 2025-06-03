using BillPay.Models.ViewModels.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.DataAccess.Repository.IRepository
{
    public interface IHomeRepo
    {
        public DashboardElements GetDashboardElements(string id);
        public IEnumerable<LineChart> GetLineTrend(string userId);
    }
}
