
using BillPay.Models;
using BillPay.Models.ViewModels.Common;
using BillPay.Models.ViewModels.Common.PredicateBuilderThis;
using BillPay.Models.ViewModels.Home;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.Models.ViewModels.Common.Expressions
{
    public class PredicateFilter
    {
        public Expression<Func<PaymentReport, bool>> predicate(List<ColumnFilterModel> Columns)
        {
            var searchCondition = PredicateBuilder.True<PaymentReport>();
            foreach (ColumnFilterModel filter in Columns)
            {
                if (!string.IsNullOrEmpty(filter.Search.Value))
                {
                    string searchValue = filter.Search.Value.Replace("(","").Replace(")", "").ToLower();
                    switch (filter.Data)
                    {
                        case ("name"):
                            searchCondition = searchCondition.And(p => p.Name.ToLower().Contains(searchValue));
                            break;
                        case ("price"):
                            searchCondition = searchCondition.And(p => p.Price.ToString().ToLower().Contains(searchValue));
                            break;
                        case ("date"):
                            searchCondition = searchCondition.And(p => p.Date.ToString().Contains(searchValue));
                            break;
                        default:
                            searchCondition = searchCondition.And(p => 1 == 1);
                            break;
                    }
                }
            }
            return searchCondition;
        }
    }
}
