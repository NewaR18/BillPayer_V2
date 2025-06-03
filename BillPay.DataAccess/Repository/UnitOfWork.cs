using BillPay.DataAccess.Data;
using BillPay.DataAccess.Repository;
using BillPay.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
		private IDbContextTransaction _transaction;
		private readonly IConfiguration _configuration;	
		public UnitOfWork(AppDbContext context, IConfiguration configuration)
        {
			_configuration = configuration;
            _context = context;
            ProductRepo = new ProductRepo(_context);
            MenuRepo = new MenuRepo(_context);
			BillSummaryRepo = new BillSummaryRepo(_context);
            ProductDetailsRepo = new ProductDetailsRepo(_context);
			BhukkadsRepo = new BhukkadsRepo(_context);
            HomeRepo = new HomeRepo(_context, _configuration);
        }
        public IProductRepo ProductRepo { get; private set; }
        public IMenuRepo MenuRepo { get; private set; }
        public IBillSummaryRepo BillSummaryRepo { get; private set; }
        public IProductDetailsRepo ProductDetailsRepo { get; private set; }
        public IBhukkadsRepo BhukkadsRepo { get; private set; }
        public IHomeRepo HomeRepo { get; private set; }

        public void BeginTransaction()
		{
			if (_transaction == null)
			{
				_transaction = _context.Database.BeginTransaction();
			}
		}

		public void CommitTransaction()
		{
			_transaction?.Commit();
		}

		public void DisposeTransaction()
		{
			_transaction?.Dispose();
		}

		public void RollbackTransaction()
		{
			_transaction?.Rollback();
		}

		public void Save()
        {
            _context.SaveChanges();
        }
    }
}
