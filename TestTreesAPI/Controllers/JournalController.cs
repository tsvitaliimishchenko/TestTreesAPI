using Microsoft.AspNetCore.Mvc;
using TestTreesAPI.DataAccess;
using TestTreesAPI.Services;

namespace TestTreesAPI.Controllers
{

    [ApiController]
    [Route("api.journal")]
    [Produces("application/json")]
    public class JournalController : ControllerBase
    {
        private readonly ExceptionLogDbContext _dbContext;
        private readonly DbLoggerService _dbLoggerService;

        public JournalController(ExceptionLogDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbLoggerService = new DbLoggerService(dbContext);
        }

        [HttpPost("getRange")]
        public IActionResult GetJournalRange(int skip, int take, JournalFilter filter)
        {
            try
            {
                var journalItems = _dbContext.ExceptionLogs
                    .Where(er => er.CreatedAt >= filter.From
                    && er.CreatedAt <= filter.To
                    && er.Data.Message.Contains(filter.Search))
                    .Skip(skip).Take(take)
                    .ToList();

                return new OkObjectResult(new
                {
                    skip,
                    count = journalItems.Count,
                    items = journalItems.Select(r => new
                    {
                        r.Id,
                        r.EventId,
                        r.CreatedAt
                    })
                });
            }
            catch (SecureException ex)
            {
                _dbLoggerService.LogException(ex, Request, ex.EventId);
                return StatusCode(500, new { type = ex.GetType().Name, id = ex.EventId.ToString(), data = new { message = ex.Message } });
            }
            catch (Exception ex)
            {
                var eventId = Guid.NewGuid();
                _dbLoggerService.LogException(ex, Request, eventId);
                return StatusCode(500, new { type = ex.GetType().Name, id = eventId.ToString(), data = new { message = $"Internal server error ID = {eventId}" } });
            }
        }

        [HttpPost("getSingle")]
        public IActionResult GetJournalSingleItem(int id)
        {
            try
            {
                var journalItem = _dbContext.ExceptionLogs.Find(id);

                return new OkObjectResult(new
                {
                    text = journalItem.Data.ToString(),
                    journalItem.Id,
                    journalItem.EventId,
                    journalItem.CreatedAt
                });
            }
            catch (SecureException ex)
            {
                _dbLoggerService.LogException(ex, Request, ex.EventId);
                return StatusCode(500, new { type = ex.GetType().Name, id = ex.EventId.ToString(), data = new { message = ex.Message } });
            }
            catch (Exception ex)
            {
                var eventId = Guid.NewGuid();
                _dbLoggerService.LogException(ex, Request, eventId);
                return StatusCode(500, new { type = ex.GetType().Name, id = eventId.ToString(), data = new { message = $"Internal server error ID = {eventId}" } });
            }
        }

        public class JournalFilter
        {
            public DateTime From { get; set; }
            public DateTime To { get; set; }
            public string Search { get; set; }
        }
    }
}
