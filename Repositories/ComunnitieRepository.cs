using Microsoft.EntityFrameworkCore;
using Reddit.Models;
using System.Linq.Expressions;

namespace Reddit.Repositories
{
    public class ComunnitieRepositoory : IComunnityRepository
    {
        private readonly ApplcationDBContext _context;

        public ComunnitieRepositoory(ApplcationDBContext applcationDBContext)
        {
            _context = applcationDBContext;
        }
        public async Task<PagedList<Community>> GetCommunities(int pageNumber = 1, int pageSize = 10, bool? isAscending = true, string sortKey = "id", string searchKey = null)
        {
            var Community = _context.Communities.AsQueryable();

            if (!isAscending == false) 
            {
                Community = Community.OrderByDescending(GetSortExpression(sortKey));
            }
            else
            {
                Community = Community.OrderBy(GetSortExpression(sortKey));

            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                Community = Community.Where(post =>
                     post.Title.Contains(searchTerm) || Community.Content.Contains(searchTerm));
            }

            return await PagedList<Post>.CreateAsync(Community, pageNumber, pageSize);
        }

        
      
        
    }
}