using Azure.Core;
using BillPay.Utilities.CommonMethods.FunctionsInterfaces;
using Hangfire;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.Utilities.BackgroundJobs.RecurringJobs
{
    public class EmailJob
    {
        public void SendEmailDaily()
        {
            RecurringJob.AddOrUpdate<IEmailFunctions>("easyjob",x => x.SendMailToDueAccounts(), Cron.Daily(11, 15));
            //RecurringJob.AddOrUpdate(, () => Console.Write("Easy!"), Cron.Daily);
        }
    }
}
