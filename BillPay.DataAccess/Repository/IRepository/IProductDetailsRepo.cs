using BillPay.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.DataAccess.Repository.IRepository
{
    public interface IProductDetailsRepo : IRepo<ProductDetails>
    {
        public void Update(ProductDetails entity);
        public new void Remove(ProductDetails entity);
        public int GetBillSummaryId(int productDetailsId);
        public void UpdateTotal(int id);
    }
}
