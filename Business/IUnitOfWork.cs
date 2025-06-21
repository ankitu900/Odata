using Business.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public interface IUnitOfWork : IDisposable
    {
        void SaveChanges();
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesInBatchAsync();
        IRepository<T> Repository<T>() where T : class;
        //void SetTokenProvider(TokenProviderType provider);
    }
}
