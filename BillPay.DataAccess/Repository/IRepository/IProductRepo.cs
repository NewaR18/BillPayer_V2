using BillPay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.DataAccess.Repository.IRepository
{
    public interface IProductRepo : IRepo<Product>
    {
        public void Update(Product entity);
    }
}
