using System;


namespace UserManagement.Web.Models.Logs
{
    public class LogEntryWithUserViewModel
    {
        public int LogId { get; set; }
        public string Action { get; set; } = default!;
        public long? UserId { get; set; }
        public string? Forename { get; set; }
        public string? Surname { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

