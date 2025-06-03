using BillPay.DataAccess.Repository;
using BillPay.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.DataAccess.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<ApplicationRole> ApplicationRole { get; set; }
        public DbSet<Menu> Menu { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<BillSummary> BillSummary { get; set; }
        public DbSet<Bhukkads> Bhukkads { get; set; }
        public DbSet<ProductDetails> ProductDetails { get; set; }
        public DbSet<ProcedureSeedingLog> ProcedureSeedingLog { get; set; }
    }
}
