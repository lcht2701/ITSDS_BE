using System.Linq.Dynamic.Core;
using X.PagedList;

namespace Persistence.Helpers
{
    public static class DataManipulationExtension
    {
        public static IQueryable<T> Filter<T>(this IQueryable<T> query, string filter)
        {
            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(filter);
            }

            return query;
        }

        public static IPagedList<T> GetPagedData<T>(this IQueryable<T> data, int page, int pageSize, string sortOrder)
        {
            // Apply sorting
            data = ApplySorting(data, sortOrder);

            // Apply paging
            return data.ToPagedList(page, pageSize);
        }

        private static IQueryable<T> ApplySorting<T>(IQueryable<T> data, string sortOrder)
        {
            if (!string.IsNullOrWhiteSpace(sortOrder))
            {
                data = data.OrderBy(sortOrder); // Use the dynamic sorting expression
            }

            return data;
        }
    }
}
