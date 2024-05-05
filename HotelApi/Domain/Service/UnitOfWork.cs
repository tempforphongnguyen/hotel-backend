using Domain.IService;
using Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Service
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDBContext _context;

        public UnitOfWork(ApplicationDBContext context)
        {
            _context = context;
        }

        public virtual void CommitChanges()
        {
            _context.SaveChanges();
        }

        public virtual async Task CommitChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
