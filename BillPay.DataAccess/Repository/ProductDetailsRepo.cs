using BillPay.DataAccess.Data;
using BillPay.DataAccess.Repository.IRepository;
using BillPay.Models;
using BillPay.Models.ViewModels.Bills;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BillPay.DataAccess.Repository
{
    public class ProductDetailsRepo : Repo<ProductDetails>, IProductDetailsRepo
    {
        private readonly AppDbContext _context;
        public ProductDetailsRepo(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(ProductDetails entity)
        {
            ProductDetails productDetails = _context.ProductDetails.FirstOrDefault(x => x.Id == entity.Id)!;
            if (productDetails != null)
            {
                productDetails.Qty = entity.Qty;
                productDetails.Price = entity.Price;
                productDetails.Total = entity.Total;
                productDetails.Discount = entity.Discount;
            }
        }
        public int GetBillSummaryId(int productDetailsId)
        {
            var query = (from bs in _context.BillSummary
                         join bk in _context.Bhukkads on bs.Id equals bk.BillSummaryId
                         join pd in _context.ProductDetails on bk.Id equals pd.BhukkadsId
                         where pd.Id == productDetailsId
                         select new
                         {
                             bs.Id
                         }).FirstOrDefault();
            return query.Id;
        }
        public void UpdateTotal(int id)
        {
            var query = (from bs in _context.BillSummary
                         join bk in _context.Bhukkads on bs.Id equals bk.BillSummaryId
                         join pd in _context.ProductDetails on bk.Id equals pd.BhukkadsId
                         where pd.Id == id
                         select new
                         {
                             BillSummaryId= bs.Id,
                             BhukkadsId=bk.Id
                         }).AsNoTracking().FirstOrDefault();
            if(query != null)
            {
                UpdateTotalOfBhukkad(query.BhukkadsId);
                UpdateTotalOfBillSummary(query.BillSummaryId);
            }
            
        }
        private void UpdateTotalOfBhukkad(int bhukkadId)
        {
            var Bhukkad = _context.Bhukkads.Where(x => x.Id.Equals(bhukkadId)).Include(x => x.Products).FirstOrDefault();
            decimal total = 0;
            foreach (var item in Bhukkad.Products)
            {
                total += item.Total;
            }
            Bhukkad.TotalOfPerson = total;
            if(total<=0)
            {
                _context.Bhukkads.Remove(Bhukkad);
            }
            _context.SaveChanges();
        }
        private void UpdateTotalOfBillSummary(int billSummaryId)
        {
            var BillSummary = _context.BillSummary.Where(x => x.Id.Equals(billSummaryId)).Include(x => x.BhukkadsList).FirstOrDefault();
            decimal totalOfBillSummary = 0;
            foreach (var item in BillSummary.BhukkadsList)
            {
                totalOfBillSummary += item.TotalOfPerson;
            }
            BillSummary.GrandTotal = totalOfBillSummary;
            if (totalOfBillSummary <= 0)
            {
                _context.BillSummary.Remove(BillSummary);
            }
            _context.SaveChanges();
        }
        public new void Remove(ProductDetails entity)
        {
            var query = (from bs in _context.BillSummary
                         join bk in _context.Bhukkads on bs.Id equals bk.BillSummaryId
                         join pd in _context.ProductDetails on bk.Id equals pd.BhukkadsId
                         where pd.Id == entity.Id
                         select new
                         {
                             BillSummaryId = bs.Id,
                             BhukkadsId = bk.Id
                         }).AsNoTracking().FirstOrDefault();
            _context.ProductDetails.Remove(entity);
            _context.SaveChanges();
            if (query != null)
            {
                UpdateTotalOfBhukkad(query.BhukkadsId);
                UpdateTotalOfBillSummary(query.BillSummaryId);
            }
        }
    }
}
