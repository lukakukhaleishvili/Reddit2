using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reddit.Dtos;
using Reddit.Mapper;
using Reddit.Models;
using Reddit.Repositories;



namespace Reddit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommunityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IComunnityRepository _comunnityRepository;

        public CommunityController(ApplicationDbContext context, IMapper mapper, IComunnityRepository comunnityRepository)
        {
            _context = context;
            _mapper = mapper;
            _comunnityRepository = comunnityRepository;

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Community>>> GetCommunities()
        {
            return await _context.Communities.ToListAsync();
        }

        public IActionResult GetCommunities(int pageNumber = 1, int pageSize = 10, bool? isAscending = true, string sortKey = "id", string searchKey = null)
        {
            try
            {
                if (pageNumber < 1)
                    pageNumber = 1;
                if (pageSize < 1)
                    pageSize = 10;


                var validSortKey = new[] { "createdAt", "postsCount", "subscribersCount", "id" };
                if (!validSortKey.Contains(sortKey))
                    sortKey = "id";


                bool ascendingOrder = isAscending ?? true;

                int skip = (pageNumber - 1) * pageSize;

                IQueriable<Community> query = _context.Communities;

                if (!string.IsNullOrEmpty(searchKey))
                {
                    query = query.Where(c => c.Name.Conatains(searchKey) || c.Description.Conatains(searchKey));
                }

                switch (sortKey)
                {
                    case "createdAt":
                        query = ascendingOrder ? query.Orderby(c => CreatedAt) : query.OrderByDescending(c => c.CreatedAt);
                        break;
                    case "postsCount":
                        query = ascendingOrder ? query.OrderBy(c => c.Posts.Count) : query.OrderByDescending(c => c.Posts.Count);
                        break;
                    case "subscribersCount":
                        query = ascendingOrder ? query.OrderBy(c => c.Subscribers.Count) : query.OrderByDescending(c => c.Subscribers.Count);
                        break;
                    default:
                        query = ascendingOrder ? query.OrderBy(c => c.Id) : query.OrderByDescending(c => c.Id);
                        break;
                }

                var comunnities = _context.Communities
                    .OrderBy(c => c.Name)
                    .Skip(skip)
                    .Take(pageSize)
                    .ToList();

                return Ok(comunnities);

            }

            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Community>> GetCommunity(int id)
        {
            var community = await _context.Communities.FindAsync(id);

            if (community == null)
            {
                return NotFound();
            }

            return community;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCommunity(CreateCommunityDto communityDto)
        {
            var community = _mapper.toCommunity(communityDto);

            await _context.Communities.AddAsync(community);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommunity(int id)
        {
            var community = await _context.Communities.FindAsync(id);
            if (community == null)
            {
                return NotFound();
            }

            _context.Communities.Remove(community);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCommunity(int id, Community community)
        {
            if (!CommunityExists(id))
            {
                return NotFound();
            }

            _context.Entry(community).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool CommunityExists(int id) => _context.Communities.Any(e => e.Id == id);
    }
}
