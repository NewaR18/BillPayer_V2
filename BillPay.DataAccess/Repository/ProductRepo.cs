using BillPay.DataAccess.Data;
using BillPay.DataAccess.Repository.IRepository;
using BillPay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.DataAccess.Repository
{
    public class ProductRepo : Repo<Product>, IProductRepo
    {
        private readonly AppDbContext _context;
        public ProductRepo(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(Product entity)
        {
            Product product = _context.Product.FirstOrDefault(u => u.Id == entity.Id);
            if (product != null)
            {
                product.Rate = entity.Rate;
                product.Name = entity.Name;
                product.QuantityType = entity.QuantityType;
            }
        }
    }
}
