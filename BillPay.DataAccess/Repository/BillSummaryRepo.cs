using BillPay.DataAccess.Data;
using BillPay.DataAccess.Repository.IRepository;
using BillPay.Models;
using BillPay.Models.ViewModels.Bills;
using BillPay.Models.ViewModels.Home;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.DataAccess.Repository
{
    public class BillSummaryRepo : Repo<BillSummary>, IBillSummaryRepo
    {
        private readonly AppDbContext _context;
        public BillSummaryRepo(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(BillSummary entity)
        {
            _context.BillSummary.Update(entity);
        }
        public new BillSummary GetFirstOrDefault(Expression<Func<BillSummary, bool>> filter, string? IncludeProperties = null)
        {
            if (IncludeProperties == null)
            {
                BillSummary billSummary = _context.BillSummary.FirstOrDefault(filter)!;
                return billSummary;
            }
            BillSummary query = _context.BillSummary.Include(x => x.BhukkadsList).ThenInclude(x => x.Products).FirstOrDefault(filter)! ;
            return query;
        }
        public ProductViewModel GetInfoBasedOnProductDetails(int id)
        {
            var query = (from bs in _context.BillSummary
                        join bk in _context.Bhukkads on bs.Id equals bk.BillSummaryId
                        join pd in _context.ProductDetails on bk.Id equals pd.BhukkadsId
                        join an in _context.ApplicationUser on bk.UserId equals an.Id
                        join p in _context.Product on pd.ProductId equals p.Id
                        where pd.Id == id
                        select new ProductViewModel()
                        {
                            BillSummaryId = bs.Id,
                            BhukkadsId = bk.Id,
                            Id = id,
                            User = an.Name,
                            Date = bs.Date,
                            ProductName = p.Name,
                            Qty = pd.Qty,
                            Price = pd.Price,
                            Discount = pd.Discount,
                            Total = pd.Total
                        }).FirstOrDefault();
            return query;
        }
        public bool UpdatePayerId(int BillSummaryId, string PayerId)
        {
            BillSummary query = _context.BillSummary.FirstOrDefault(x => x.Id.Equals(BillSummaryId))! ;
            if(query == null)
            {
                return false;
            }
            query.PayerUserId = PayerId;
            _context.SaveChanges();
            return true;
        }
        public bool UpdateDate(int BillSummaryId, DateTime DateEaten)
        {
            BillSummary query = _context.BillSummary.FirstOrDefault(x => x.Id.Equals(BillSummaryId))!;
            if (query == null)
            {
                return false;
            }
            query.Date = DateEaten;
            _context.SaveChanges();
            return true;
        }
        public bool UpdateGrandTotal(int BillSummaryId)
        {
            BillSummary query = _context.BillSummary.Where(x => x.Id.Equals(BillSummaryId)).Include(x=>x.BhukkadsList).FirstOrDefault()!;
            if (query == null)
            {
                return false;
            }
            decimal total = 0;
            foreach(var item in query.BhukkadsList)
            {
                total += item.TotalOfPerson;
            }
            query.GrandTotal = total;
            _context.SaveChanges();
            return true;
        }

        public ReportDetails GetPaymentFormByBillSummaryId(int billSummaryId, bool paid)
        {
            var reportDetails = (from bs in _context.BillSummary
                                 join bk in _context.Bhukkads on bs.Id equals bk.BillSummaryId
                                 join payerUser in _context.ApplicationUser on bs.PayerUserId equals payerUser.Id
                                 join bhukkadUser in _context.ApplicationUser on bk.UserId equals bhukkadUser.Id
                                 where bk.Paid == paid && bs.Id == billSummaryId
                                 select new
                                 {
                                     Payer = payerUser.Name,
                                     Date = bs.Date.Date,
                                     Bhukkad = bhukkadUser.Name,
                                     TotalOfPerson = bk.TotalOfPerson
                                 }).ToList();

            var result = reportDetails.GroupBy(
                rd => new { rd.Payer, rd.Date },
                (key, group) => new ReportDetails
                {
                    Payer = key.Payer,
                    Date = key.Date,
                    GrandTotal = group.Sum(x => x.TotalOfPerson).ToString(),
                    BhukkadsTotal = group.Select(x => new BhukkadTotal
                    {
                        Bhukkad = x.Bhukkad,
                        Total = x.TotalOfPerson
                    }).ToList()
                }).FirstOrDefault();
            return result;
        }
    }
}
