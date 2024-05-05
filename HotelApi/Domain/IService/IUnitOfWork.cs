using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IService
{
    public interface IUnitOfWork
    {
        void CommitChanges();
        Task CommitChangesAsync();
    }
}
