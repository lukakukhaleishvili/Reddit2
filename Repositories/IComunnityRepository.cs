using Reddit.Models;
namespace Reddit.Repositories
{
    public interface IComunnityRepository
    {
        public Task<PagedList<Community>> GetCommunities(int pageNumber = 1, int pageSize = 10, bool? isAscending = true, string sortKey = "id", string searchKey = null);
    }
}
