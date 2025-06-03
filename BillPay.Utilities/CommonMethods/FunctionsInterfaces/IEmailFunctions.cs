using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.Utilities.CommonMethods.FunctionsInterfaces
{
    public interface IEmailFunctions
    {
        public void SendMailToDueAccounts();
    }
}
