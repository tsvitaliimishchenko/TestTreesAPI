using Azure.Core;
using Microsoft.EntityFrameworkCore;
using TestTreesAPI.DataAccess;
using TestTreesAPI.Models;

namespace TestTreesAPI.Services
{
    public class DbLoggerService
    {
        private readonly ExceptionLogDbContext _dbContext;
        public DbLoggerService(ExceptionLogDbContext dbContext) 
        {
            _dbContext = dbContext;
        }
        public void LogException(Exception ex, HttpRequest request, Guid eventId)
        {
            var log = new ExceptionLog
            {
                EventId = eventId,
                CreatedAt = DateTime.UtcNow,
                Type = ex.GetType().Name,
                Data = new ExceptionLogData
                {
                    QueryParameters = request.QueryString.ToString(),
                    BodyParameters = request.Body.ToString(),
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                }
            };
            _dbContext.ExceptionLogs.Add(log);
            _dbContext.SaveChanges();
        }
    }
}
