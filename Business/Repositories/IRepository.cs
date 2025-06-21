using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repositories
{
    public interface IRepository<T> : IDisposable where T : class
    {
        void Add(T entity);

        void Update(T entity);

        void AddLink(object entity, string sourceProperty, object relatedObject);

        void DeleteLink(T entity, string sourceProperty, object relatedObject);

        void DeleteLinks(T entity, string sourceProperty);

        void Remove(T entity);

        IEnumerable<T> Url(string url);

        int Count(string url);

        IQueryable<T> All();

        IQueryable<T> All(params Expression<Func<T, object>>[] includeProperties);

        IQueryable<T> All(params string[] includes);

        T FindSingle(Expression<Func<T, bool>> predicate = null,

        params Expression<Func<T, object>>[] includes);

        IQueryable<T> Filter(Expression<Func<T, bool>> predicate);

        IQueryable<T> Filter(Expression<Func<T, bool>> filter, out int total, int index = 0, int size = 50);

        IQueryable<T> Find(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includes);

        IQueryable<T> FindIncluding(params Expression<Func<T, object>>[] includeProperties);

        IQueryable<T> FindIncluding(Expression<Func<T, bool>> predicate = null, params string[] includes);

        int Count(Expression<Func<T, bool>> predicate = null);

        bool Contains(Expression<Func<T, bool>> predicate);

        bool Exist(Expression<Func<T, bool>> predicate = null);

        //PagedListResult<T> Search(SearchQuery<T> searchQuery);

        //PagedListResult<T> Search(SearchQuery<T> searchQuery, out string query);

        //asyc methods
        Task<List<T>> AllAsync();

        Task<T> FindSingleAsync(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includes);

        Task<List<T>> FilterAsync(Expression<Func<T, bool>> predicate);

        Task<List<T>> FilterAsync(Expression<Func<T, bool>> filter, int index = 0, int size = 50);

        Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includes);

        Task<List<T>> FindIncludingAsync(params Expression<Func<T, object>>[] includeProperties);

        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);

        Task<bool> ContainsAsync(Expression<Func<T, bool>> predicate);

        Task<bool> ExistAsync(Expression<Func<T, bool>> predicate = null);

        void DetachObject(T entity);

        void DetachObject(IEnumerable<T> entities);

        void RefreshCache();

        ODataPatchQuery<T> PatchQuery(System.Linq.Expressions.Expression<Func<T, bool>> predicate);

        Task<bool> SaveChangesAsync();

        bool SaveChanges();
    }
}