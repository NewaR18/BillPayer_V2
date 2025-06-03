using BillPay.DataAccess.Repository.IRepository;
using BillPay.Models;
using BillPay.Models.ViewModels.Bills;
using BillPay.Utilities.CommonMethods.FunctionsInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.Utilities.CommonMethods
{
    public class EmailFunctions : IEmailFunctions
    {
        private readonly IUnitOfWork _repo;
        private readonly IEmailSender _emailSender;
        public EmailFunctions(IUnitOfWork repo,IEmailSender emailSender) 
        {
            _repo = repo;
            _emailSender = emailSender;
        }
        public void SendMailToDueAccounts()
        {
            IEnumerable<PaymentDueAccounts> dueAccounts = _repo.BhukkadsRepo.GetDueAccounts();
            //IEnumerable<PaymentDueAccounts> dueAccounts = new List<PaymentDueAccounts>()
            //{
            //    new PaymentDueAccounts
            //    {
            //        Name = "Sudip Shrestha",
            //        Email = "sudipshrestha960@gmail.com",
            //        TotalDue = 0,
            //    }
            //};
            if (dueAccounts != null)
            {
                foreach(var dueAccount in dueAccounts)
                {
                    string Subject = "Payment Due";
                    string htmlMessage = $"<div>Dear {dueAccount.Name},</div><br/><div>Your account is due with Rs {dueAccount.TotalDue}.</div><div>Please check BillPayer app for more details.</div><div><i>(Automated Email)</i></div><br/><br/><div>Best Regards,</div><div>BillPayer</div>";
                    _emailSender.SendEmailAsync(dueAccount.Email, Subject, htmlMessage);
                }
            }
        }
    }
}
