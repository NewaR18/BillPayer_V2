using BillPay.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        public IProductRepo ProductRepo { get; }
        public IBillSummaryRepo BillSummaryRepo { get; }
        public IMenuRepo MenuRepo { get; }
        public IProductDetailsRepo ProductDetailsRepo { get; }
        public IBhukkadsRepo BhukkadsRepo { get; }
        public IHomeRepo HomeRepo { get; }
        void Save();
		void BeginTransaction();
		void CommitTransaction();
		void RollbackTransaction();
        void DisposeTransaction();
	}
}
