namespace OcelotApiGateWay.Entites
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string RequestPath { get; set; }
        public string RequestBody { get; set; }
        public string Response { get; set; }
        public int ResponseCode { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
