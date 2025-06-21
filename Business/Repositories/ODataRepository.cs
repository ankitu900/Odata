using Microsoft.OData.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Business.Repositories
{
    public class ODataRepository<T> : IRepository<T> where T : class
    {
        private DataServiceContext _odataContext;
        string _entityName = null;

        public ODataRepository(DataServiceContext container)
        {
            _entityName = typeof(T).Name;
            _odataContext = container;
        }

        public IEnumerable<T> Url(string query)
        {
            _odataContext.MergeOption = MergeOption.NoTracking;
            return _odataContext.Execute<T>(new Uri(_odataContext.BaseUri, query)).ToList();
        }

        public int Count(string query)
        {
            _odataContext.MergeOption = MergeOption.NoTracking;
            return _odataContext.Execute<int>(new Uri(_odataContext.BaseUri, query), "GET").FirstOrDefault();
        }

        public IQueryable<T> All()
        {
            return GetEntity();
        }

        public IQueryable<T> All(params Expression<Func<T, object>>[] includeProperties)
        {
            var set = addIncludeProperties(GetEntity(), includeProperties);
            return set.AsQueryable();
        }

        public IQueryable<T> All(params string[] includeProperties)
        {
            var set = addIncludeProperties(GetEntity(), includeProperties);
            return set.AsQueryable();
        }

        public void Add(T entity)
        {
            _odataContext.AddObject(_entityName, entity);
        }

        public void Update(T entity)
        {
            AttachObject(entity);
            _odataContext.UpdateObject(entity);
        }

        public void AddLink(object entity, string sourceProperty, object relatedObject)
        {
            if (!_odataContext.Links.Contains(_odataContext.GetLinkDescriptor(entity, sourceProperty, relatedObject)))
            {
                _odataContext.AddLink(entity, sourceProperty, relatedObject);
            }
        }

        public void DeleteLink(T entity, string sourceProperty, object relatedObject)
        {
            if (_odataContext.Links.Contains(_odataContext.GetLinkDescriptor(entity, sourceProperty, relatedObject)))
            {
                _odataContext.DeleteLink(entity, sourceProperty, relatedObject);
            }
        }

        public void DeleteLinks(T entity, string sourceProperty)
        {
            foreach (LinkDescriptor linkDescriptor in _odataContext.Links)
            {
                if (linkDescriptor.Source == entity && linkDescriptor.SourceProperty == sourceProperty)
                {
                    _odataContext.DeleteLink(entity, sourceProperty, linkDescriptor.Target);
                }
            }
        }

        public void Remove(T entity)
        {
            AttachObject(entity);
            _odataContext.DeleteObject(entity);
            var response = _odataContext.SaveChangesAsync().Result;
            ExecuteResponse(response);
        }

        public void AttachObject(T entity)
        {
            if (!_odataContext.EntityTracker.Entities.Contains(_odataContext.GetEntityDescriptor(entity)))
                _odataContext.AttachTo(_entityName, entity);
        }

        public async Task<bool> SaveChangesAsync()
        {
            var result = await _odataContext.SaveChangesAsync();
            ExecuteResponse(result);
            return true;
        }

        public bool SaveChanges()
        {
            var result = _odataContext.SaveChanges();
            ExecuteResponse(result);
            return true;
        }

        public T FindSingle(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includes)
        {
            var set = FindIncluding(includes);
            return (predicate == null) ?
                   set.FirstOrDefault() :
                   set.FirstOrDefault(predicate);
        }

        public IQueryable<T> Find(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includes)
        {
            var set = FindIncluding(includes);
            return (predicate == null) ? set : set.Where(predicate);
        }

        public IQueryable<T> FindIncluding(Expression<Func<T, bool>> predicate = null, params string[] includes)
        {
            var set = FindIncluding(includes);
            return (predicate == null) ? set : set.Where(predicate);
        }

        public IQueryable<T> FindIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            var set = addIncludeProperties(GetEntity(), includeProperties);
            return set.AsQueryable();
        }

        public IQueryable<T> FindIncluding(string[] includeProperties)
        {
            var set = addIncludeProperties(GetEntity(), includeProperties);
            return set.AsQueryable();
        }

        private DataServiceQuery<T> addIncludeProperties(DataServiceQuery<T> set, params Expression<Func<T, object>>[] includeProperties)
        {
            if (includeProperties != null)
            {
                foreach (var include in includeProperties)
                {
                    set = set.Expand(include);
                }
            }
            return set;
        }

        private DataServiceQuery<T> addIncludeProperties(DataServiceQuery<T> set, params string[] includeProperties)
        {
            if (includeProperties != null)
            {
                foreach (var include in includeProperties)
                {
                    set = set.Expand(include);
                }
            }
            return set;
        }

        public IQueryable<T> Filter(Expression<Func<T, bool>> predicate)
        {
            return GetEntity().Where(predicate);
        }

        public IQueryable<T> Filter(Expression<Func<T, bool>> filter, out int total, int index = 0, int size = 50)
        {
            var DbSet = GetEntity();

            int skipCount = index * size;
            var _resetSet = filter != null ? DbSet.Where(filter).AsQueryable() : DbSet.AsQueryable();
            _resetSet = skipCount == 0 ? _resetSet.Take(size) : _resetSet.Skip(skipCount).Take(size);
            total = _resetSet.Count();
            return _resetSet.AsQueryable();
        }

        public int Count(Expression<Func<T, bool>> predicate = null)
        {
            var set = GetEntity();
            return (predicate == null) ?
                   set.Count() :
                   set.Count(predicate);
        }

        public bool Contains(Expression<Func<T, bool>> predicate)
        {
            return Count(predicate) > 0;
        }

        public bool Exist(Expression<Func<T, bool>> predicate = null)
        {
            var set = GetEntity();
            var response = (predicate == null) ? set.Any() : set.Any(predicate);
            return response;
        }

        //public Models.DataTables.PagedListResult<T> Search(SearchQuery<T> searchQuery)
        //{
        //    return All().Search<T>(searchQuery);
        //}

        //public Models.DataTables.PagedListResult<T> Search(SearchQuery<T> searchQuery, out string query)
        //{
        //    return All().Search<T>(searchQuery, out query);
        //}


        //Async Methods
        public async Task<List<T>> AllAsync()
        {
            return await Task.FromResult(All().ToList());
        }

        public async Task<T> FindSingleAsync(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includes)
        {
            return await Task.FromResult(FindSingle(predicate, includes));
        }

        public async Task<List<T>> FilterAsync(Expression<Func<T, bool>> predicate)
        {
            return await Task.FromResult(Filter(predicate).ToList());
        }

        public async Task<List<T>> FilterAsync(Expression<Func<T, bool>> filter, int index = 0, int size = 50)
        {
            int total = 0;
            return await Task.FromResult(Filter(filter, out total, index, size).ToList());
        }

        public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includes)
        {
            return await Task.FromResult(Find(predicate, includes).ToList());
        }

        public async Task<List<T>> FindIncludingAsync(params Expression<Func<T, object>>[] includeProperties)
        {
            return await Task.FromResult(FindIncluding(includeProperties).ToList());
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
        {
            return await Task.FromResult(Count(predicate));
        }

        public async Task<bool> ContainsAsync(Expression<Func<T, bool>> predicate)
        {
            return await Task.FromResult(Contains(predicate));
        }

        public async Task<bool> ExistAsync(Expression<Func<T, bool>> predicate = null)
        {
            return await Task.FromResult(Exist(predicate));
        }

        public void DetachObject(T entity)
        {
            if (entity != null)
                if (_odataContext.EntityTracker.Entities.Contains(_odataContext.GetEntityDescriptor(entity)))
                    _odataContext.Detach(entity);
        }

        public void DetachObject(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                DetachObject(entity);
            }
        }

        public void DetachObject<TEntity>(TEntity entity)
        {
            if (entity != null)
                if (_odataContext.EntityTracker.Entities.Contains(_odataContext.GetEntityDescriptor(entity)))
                    _odataContext.Detach(entity);
        }

        public void DetachObject<TEntity>(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                DetachObject(entity);
            }
        }

        public void RefreshCache()
        {
            foreach (var item in _odataContext.Entities)
            {
                DetachObject(item.Entity);
            }
        }

        public ODataPatchQuery<T> PatchQuery(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            this._odataContext.MergeOption = MergeOption.AppendOnly;
            this.RefreshCache();
            var query = this.All().Where(predicate);
            return new ODataPatchQuery<T>(this, query);
        }

        public void Dispose()
        {
            if (_odataContext != null)
            {
                _odataContext = null;
            }
        }

        #region helpers

        private void ExecuteResponse(DataServiceResponse response)
        {
            var error = response.Where(a => a.Error != null).Select(a => a.Error).FirstOrDefault();
            if (error != null)
                throw error;
        }

        public DataServiceQuery<T> GetEntity()
        {
            //var type = typeof(DataServiceQuery<T>);

                var entityProperty = _odataContext
                    .GetType()
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    //.Where(a => a.DeclaringType == type)
                    .Where(a => a.Name == _entityName)
                    .FirstOrDefault();

                if (entityProperty == null)
                {
                    throw new NotSupportedException("Entity not found");
                }

            DataServiceQuery<T> entity = (DataServiceQuery<T>)entityProperty.GetValue(_odataContext);
            return entity;
        }

        #endregion
    }

    public class ODataPatchQuery<T> where T : class
    {
        private readonly IRepository<T> repository;
        private readonly DataServiceCollection<T> data;

        public ODataPatchQuery(IRepository<T> repository, IQueryable<T> data)
        {
            this.repository = repository;
            this.data = new DataServiceCollection<T>(data);
        }

        public void Set(Action<T> action)
        {
            for (int i = 0; i < data.Count; i++)
            {
                action(data[i]);
            }
        }

        public async Task SaveChangesAsync()
        {
            await repository.SaveChangesAsync();
        }

        public void SaveChanges()
        {
            repository.SaveChanges();
        }
    }
}
