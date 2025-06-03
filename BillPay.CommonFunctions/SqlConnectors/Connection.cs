using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.CommonFunctions.SqlConnectors
{
    public class Connection
    {
        private IConfigurationRoot Configuration { get; set; }
        public string GetConnectionString()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            Configuration = configuration.Build();
            string constr1 = Configuration.GetConnectionString("myconnection");
            return constr1;
        }
    }
}
