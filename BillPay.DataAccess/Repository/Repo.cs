﻿
using BillPay.DataAccess.Data;
using BillPay.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.DataAccess.Repository
{
    public class Repo<T> : IRepo<T> where T : class
    {
        private readonly AppDbContext _context;
        internal DbSet<T> DbSet;
        public Repo(AppDbContext context) { 
            _context = context;
            this.DbSet = _context.Set<T>();
        }
        public void Add(T entity)
        {
            DbSet.Add(entity);
        }
        public IEnumerable<T> GetAll(Expression<Func<T,bool>>? filter = null, string? IncludeProperties = null)
        {
            IQueryable<T> query = DbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (IncludeProperties != null)
            {
                foreach(var property in IncludeProperties.Split(new char[] {','},StringSplitOptions.RemoveEmptyEntries)) 
                {
                    query = query.Include(property);
                }
            }
            return query;
        }
        public T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? IncludeProperties = null)
        {
            IQueryable<T> query = DbSet.Where(filter);
            if (IncludeProperties != null)
            {
                foreach (var property in IncludeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            return query.FirstOrDefault()!;
        }
        public void Remove(T entity)
        {
            DbSet.Remove(entity);
        }
        public void RemoveRange(IEnumerable<T> entities)
        {
            DbSet.RemoveRange(entities);
        }
        public int GetCount()
        {
            return DbSet.Count();
        }
    }
}
