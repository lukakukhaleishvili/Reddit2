using Microsoft.EntityFrameworkCore;
namespace Reddit.Repositories
{
    public class PagedList<T>
    {

        private PagedList(PagedList<T> items,int pageNumber, int pageSize, int count, bool hasNextpage, bool hasPreviousPage)
        {
            items = items;
            pageNumber = pageNumber;
            pageSize = pageSize;
            count = count;
            hasNextpage = hasNextpage;
            hasPreviousPage = hasPreviousPage;
        }


        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public List<T> Items { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set;}



        public static async Task<PagedList<T>> CreateAsync(IQueriable<T> items, int pageNumber, int pageSize, )
        {
            var pagedItems = await items.skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            var totalCount = await items.CountAsync();
            var hasNectPage = (pageNumber *  pageSize) < totalCount;
            var hasPreviousPage = pageNumber > 1;


            return new PagedList<T>(pagedItems, pageNumber, pageSize, totalCount, hasNectPage, hasPreviousPage);
        }
    }
}
