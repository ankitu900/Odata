using Business.Helper;
using Business.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class Service<T> : IService<T> where T : class
    {
        protected readonly IUnitOfWork unitOfWork;

        public Service(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public void Detach<TEntity>(TEntity entity) where TEntity : class
        {
            unitOfWork.Repository<TEntity>().DetachObject(entity);
        }

        public void Detach<TEntity>(IEnumerable<TEntity> entity) where TEntity : class
        {
            unitOfWork.Repository<TEntity>().DetachObject(entity);
        }

        //public PagedListResult<T> Search(SearchQuery<T> searchQuery)
        //{
        //    return unitOfWork.Repository<T>().Search(searchQuery);
        //}

        //public virtual void SetTokenProvider(TokenProviderType provider)
        //{
        //    unitOfWork.SetTokenProvider(provider);
        //}

        public IService<T> NoTrack()
        {
            unitOfWork.Repository<T>().All().AsDataServiceQuery().ODataNoTracking();
            return this;
        }

        public void RefreshCache()
        {
            unitOfWork.Repository<T>().RefreshCache();
        }

        public ODataPatchQuery<T> PatchQuery(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            return unitOfWork.Repository<T>().PatchQuery(predicate);
        }

        //public T QueryData<T>(string query, Dictionary<string, string> sqlParameters) where T : class
        //{
        //    var parm = new QueryDataRequest
        //    {
        //        Query = query
        //    };

        //    foreach (var item in sqlParameters)
        //    {
        //        parm.Parameters.Add(new QueryDataParameter
        //        {
        //            Key = item.Key,
        //            Value = item.Value
        //        });
        //    }

        //    var result = this.unitOfWork.Repository<Dashboard>().All()
        //        .AsDataServiceQuery()
        //        .QueryData(parm)
        //        .GetValue();

        //    return result.Json.DeserializeJson<T>();
        //}

        public async Task SaveChangesAsync()
        {
            await unitOfWork.Repository<T>().SaveChangesAsync();
        }
    }
}
