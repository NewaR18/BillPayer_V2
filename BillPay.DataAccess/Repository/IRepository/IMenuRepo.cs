using BillPay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.DataAccess.Repository.IRepository
{
    public interface IMenuRepo:IRepo<Menu>
    {
        public void Update(Menu menu);
    }
}
