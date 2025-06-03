using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.DataAccess.DbInitializers
{
    public interface IDbInitializer
    {
        public void Initialize();
    }
}
