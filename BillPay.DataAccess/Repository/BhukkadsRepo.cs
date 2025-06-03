using BillPay.DataAccess.Data;
using BillPay.DataAccess.Repository.IRepository;
using BillPay.Models;
using BillPay.Models.ViewModels.Bills;
using BillPay.Models.ViewModels.Common;
using BillPay.Models.ViewModels.Common.Expressions;
using BillPay.Models.ViewModels.Common.PredicateBuilderThis;
using BillPay.Models.ViewModels.Home;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.DataAccess.Repository
{
    public class BhukkadsRepo : Repo<Bhukkads>, IBhukkadsRepo
    {
        private readonly AppDbContext _context;
        public BhukkadsRepo(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(Bhukkads entity)
        {
            _context.Bhukkads.Update(entity);
        }
        public AddProductDetailsViewModel GetInfoBasedOnBhukkadsId(int bhukkadsId)
        {
            var query = (from bs in _context.BillSummary
                         join bk in _context.Bhukkads on bs.Id equals bk.BillSummaryId
                         join an in _context.ApplicationUser on bk.UserId equals an.Id
                         where bk.Id == bhukkadsId
                         select new AddProductDetailsViewModel()
                         {
                             BillSummaryId = bs.Id,
                             BhukkadsId = bk.Id,
                             User = an.Name,
                             Date = bs.Date,
                             ProductDetails = new ProductDetails()
                         }).FirstOrDefault();
            return query;
        }
        public IEnumerable<UnpaidBhukkads> GetUnpaidBhukkadListForDashboard(string userId)
        {
            var query = ( from bs in _context.BillSummary
                          join bk in _context.Bhukkads on bs.Id equals bk.BillSummaryId
                         join au in _context.ApplicationUser on bk.UserId equals au.Id
                         where au.Id == userId && bk.Paid == false
                         orderby bk.Id
                         select new UnpaidBhukkads
                         {
                             BhukkadsId = bk.Id,
                             Date = bs.Date,
                             Price = bk.TotalOfPerson
                         });
            return query;
        }

        public PaymentDetails GetPaymentFormByBhukkadsId(int bhukkadsId)
        {
            PaymentDetails query = (from bs in _context.BillSummary
                                    join bk in _context.Bhukkads on bs.Id equals bk.BillSummaryId
                                    join anu in _context.ApplicationUser on bs.PayerUserId equals anu.Id
                                    where bk.Id == bhukkadsId
                                    select new PaymentDetails
                                    {
                                        BhukkadsId = bhukkadsId,
                                        Name = anu.Name,
                                        EsewaName = anu.EsewaName,
                                        EsewaPhone = anu.EsewaPhone,
                                        TotalOfPerson = bk.TotalOfPerson,
                                        ProductListOfBhukkad = (from pd in _context.ProductDetails
                                                                join p in _context.Product on pd.ProductId equals p.Id
                                                                where pd.BhukkadsId == bk.Id
                                                                select new ProductOfBhukkad
                                                                {
                                                                    Name = p.Name,
                                                                    Amount = pd.Total
                                                                }).ToList()
                                    }).FirstOrDefault()!;
            return query;
        }

        public bool UpdatePaymentStatus(int bhukkadsId)
        {
            Bhukkads bhukkads = _context.Bhukkads.FirstOrDefault(x=>x.Id.Equals(bhukkadsId))!;
            if(bhukkads != null)
            {
                bhukkads.Paid = true;
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        public IEnumerable<PaymentDueAccounts> GetDueAccounts()
        {
            IEnumerable<PaymentDueAccounts> dueAccounts = (from bk in _context.Bhukkads 
                               join anu in _context.ApplicationUser on bk.UserId equals anu.Id
                               where bk.Paid == false
                               group new { bk, anu } by new { bk.UserId, anu.Name, anu.Email } into g
                               select new PaymentDueAccounts
                               {
                                   UserId = g.Key.UserId,
                                   Name = g.Key.Name,
                                   Email = g.Key.Email,
                                   TotalDue = g.Sum(x => x.bk.TotalOfPerson)
                               }).ToList();
            return dueAccounts;
        }

        public int GetCountOfUser(string userId)
        {
            return _context.Bhukkads.Where(s => s.UserId.Equals(userId)).Count();
        }
        public PaginatedPaymentReport GetPaginatedRows(PaginationViewModel dataTableAjaxModel, ClaimsPrincipal User)
        {
            PredicateFilter _predicate = new PredicateFilter();
            var searchCondition = PredicateBuilder.True<PaymentReport>();
            bool ShowAll = false;
            if (User.IsInRole("Admin") || User.IsInRole("Super Admin"))
            {
                ShowAll = true;
            }
            IQueryable<PaymentReport> allData;
            
            if (dataTableAjaxModel.Status !=null)
            {
                switch (dataTableAjaxModel.Status.ToLower())
                {
                    case "topay":
                        allData = GetExpenseOrDueReport();
                        searchCondition =UserSpecificShowData(ShowAll, User, searchCondition);
                        searchCondition = searchCondition.And(p => p.Paid == false);
                        break;
                    case "toreceive":
                        allData = GetReceivableReport(false,User);
                        break;
                    case "received":
                        allData = GetReceivableReport(true,User);
                        break;
                    default:
                        allData = GetExpenseOrDueReport();
                        searchCondition = UserSpecificShowData(ShowAll, User, searchCondition);
                        searchCondition = searchCondition.And(p => p.Paid == true);
                        break;
                }
            }
            else
            {
                allData = GetExpenseOrDueReport();
            }
            allData = allData.Where(_predicate.predicate(dataTableAjaxModel.Columns)).Where(searchCondition);
            var paginatedData = allData.Skip(dataTableAjaxModel.Start).Take(dataTableAjaxModel.Length);//.AsNoTracking();
            PaginatedPaymentReport paginatedPaymentReport = new PaginatedPaymentReport()
            {
                Data = paginatedData,
                TotalCount = allData.Count()
            };
            return paginatedPaymentReport;
        }
        public Expression<Func<PaymentReport, bool>> UserSpecificShowData(bool ShowAll, ClaimsPrincipal User, Expression<Func<PaymentReport, bool>> searchCondition)
        {
            if (!ShowAll)
            {
                string userId=GetUserId(User);
                searchCondition = searchCondition.And(s => s.UserId.Equals(userId));
            }
            return searchCondition;
        }
        public string GetUserId(ClaimsPrincipal User)
        {
            var userClaimsIdentity = (ClaimsIdentity)User.Identity!;
            var userIdClaim = userClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(userIdClaim != null)
            {
                return userIdClaim.Value;
            }
            return "";
        }
        public IQueryable<PaymentReport> GetReceivableReport(bool paid,ClaimsPrincipal User)
        {
            IQueryable<PaymentReport> allData;
            allData = (from bs in _context.BillSummary
                       join bk in _context.Bhukkads on bs.Id equals bk.BillSummaryId
                       join anu in _context.ApplicationUser on bs.PayerUserId equals anu.Id
                       where bk.Paid == paid && bs.PayerUserId == GetUserId(User)
                       group new { bs, anu, bk } by new { bs.Id, bs.PayerUserId, anu.Name, bs.Date } into g
                       select new PaymentReport
                       {
                           UserId = "",
                           Name = g.Key.Name,
                           Date = g.Key.Date,
                           Price = g.Sum(x => x.bk.TotalOfPerson),
                           Paid = paid,
                           PayerUserId = g.Key.PayerUserId,
                           BillSummaryId = g.Key.Id
                       });
            return allData;
        }
        public IQueryable<PaymentReport> GetExpenseOrDueReport()
        {
            IQueryable<PaymentReport> allData;
            allData = (from bs in _context.BillSummary
                       join bk in _context.Bhukkads on bs.Id equals bk.BillSummaryId
                       join anu in _context.ApplicationUser on bk.UserId equals anu.Id
                       select new PaymentReport
                       {
                           UserId = bk.UserId,
                           Name = anu.Name,
                           Date = bs.Date,
                           Price = bk.TotalOfPerson,
                           Paid = bk.Paid,
                           PayerUserId = bs.PayerUserId,
                           BillSummaryId = bs.Id
                       });
            return allData;
        }
        public int GetToReceiveCount(string userId)
        {
            int count = (from bs in _context.BillSummary
                         join bk in _context.Bhukkads on bs.Id equals bk.BillSummaryId
                         where bs.PayerUserId == userId && bk.Paid == false
                         group bs by bs.Id into g
                         select 1
                         ).Count();
            return count;
        }

        public int GetReceivedCount(string userId)
        {
            int count = (from bs in _context.BillSummary
                         join bk in _context.Bhukkads on bs.Id equals bk.BillSummaryId
                         where bs.PayerUserId == userId && bk.Paid == true
                         group bs by bs.Id into g
                         select 1
                         ).Count();
            return count;
        }
    }
}
