using OcelotApiGateWay.Context;
using OcelotApiGateWay.Entites;

namespace OcelotApiGateWay.Tasks
{
    public class AuditLogJob
    {
        private readonly HangFireContext _db;
        private readonly ILogger<AuditLogJob> _logger;

        public AuditLogJob(HangFireContext db, ILogger<AuditLogJob> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task SaveAsync(string requestPath, string requestBody, string responseBody, int responseCode)
        {
            var log = new AuditLog
            {
                RequestPath = requestPath,
                RequestBody = requestBody,
                Response = responseBody,
                ResponseCode = responseCode
            };
            _db.AuditLogs.Add(log);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Saved AuditLog {Id}", log.Id);
        }
    }
}
