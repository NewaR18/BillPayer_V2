using BillPay.Utilities.BackgroundJobs.RecurringJobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.Utilities.Modules
{
    public static class StartScheduler
    {
        public static void StartHangfireSchedule(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var emailJob = scope.ServiceProvider.GetRequiredService<EmailJob>();
                emailJob.SendEmailDaily();
            }
        }
    }
}
