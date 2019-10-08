using System;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.Data;
using ConferenceDTO;
using ConferenceDTO.ConferenceDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public SearchController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Search(SearchTerm term)
        {
            var query = term.Query;

            var sessionResultsTask = _db.Sessions.AsNoTracking()
                                                   .Include(s => s.Track)
                                                   .Include(s => s.SessionSpeakers)
                                                     .ThenInclude(ss => ss.Speaker)
                                                   .Where(s =>
                                                       s.Title.Contains(query, StringComparison.InvariantCultureIgnoreCase) ||
                                                       s.Track.Name.Contains(query, StringComparison.InvariantCultureIgnoreCase)
                                                   )
                                                   .ToListAsync();

            var speakerResultsTask = _db.Speakers.AsNoTracking()
                                                   .Include(s => s.SessionSpeakers)
                                                     .ThenInclude(ss => ss.Session)
                                                   .Where(s =>
                                                       s.Name.Contains(query, StringComparison.InvariantCultureIgnoreCase) ||
                                                       s.Bio.Contains(query, StringComparison.InvariantCultureIgnoreCase) ||
                                                       s.WebSite.Contains(query, StringComparison.InvariantCultureIgnoreCase)
                                                   )
                                                   .ToListAsync();

            var sessionResults = await sessionResultsTask;
            var speakerResults = await speakerResultsTask;

            var results = sessionResults.Select(s => new SearchResult
            {
                Type = SearchResultType.Session,
                Value = JObject.FromObject(s.MapSessionResponse())
            })
            .Concat(speakerResults.Select(s => new SearchResult
            {
                Type = SearchResultType.Speaker,
                Value = JObject.FromObject(s.MapSpeakerResponse())
            }));

            return Ok(results);
        }
    }
}
