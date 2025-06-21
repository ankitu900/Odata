using Business.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public interface IService<T> where T : class
    {
        void Detach<TEntity>(TEntity entity) where TEntity : class;
        void Detach<TEntity>(IEnumerable<TEntity> entity) where TEntity : class;
        //PagedListResult<T> Search(SearchQuery<T> searchQuery);
        //void SetTokenProvider(TokenProviderType tokenProvider);
        IService<T> NoTrack();
        void RefreshCache();
        Task SaveChangesAsync();
        ODataPatchQuery<T> PatchQuery(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
        //T QueryData<T>(string query, Dictionary<string, string> sqlParameters) where T : class;
    }
}
