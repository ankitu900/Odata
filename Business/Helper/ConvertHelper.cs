using Microsoft.OData.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Helper
{
    public static class ConvertHelper
    {
        public static DataServiceQuery<T> AsDataServiceQuery<T>(this IQueryable<T> queryable)
        {
            return queryable as DataServiceQuery<T>;
        }
        public static DataServiceQuery<T> ODataNoTracking<T>(this DataServiceQuery<T> queryable)
        {
            queryable.Context.MergeOption = MergeOption.NoTracking;
            return queryable;
        }
    }
}
