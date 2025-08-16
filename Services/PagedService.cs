using ProvaPub.Models;
using ProvaPub.Repository;
using Microsoft.EntityFrameworkCore;

namespace ProvaPub.Services
{
    public class PagedService
    {
        TestDbContext _ctx;
        public PagedService(TestDbContext ctx)
        {
            _ctx = ctx;
        }

        public PagedList<T> GetPagedList<T>(IQueryable<T> query, int page, int pageSize = 10)
        {
            var totalCount = query.Count();
            var items = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            bool hasNext = (page * pageSize) < totalCount;

            return new PagedList<T>
            {
                Items = items,
                TotalCount = totalCount,
                HasNext = hasNext
            };
        }
    }
}