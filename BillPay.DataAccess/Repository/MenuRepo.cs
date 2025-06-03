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
    public class MenuRepo : Repo<Menu>, IMenuRepo
    {
        private readonly AppDbContext _context;
        public MenuRepo(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(Menu menu)
        {
            _context.Menu.Update(menu);
        }
    }
}
