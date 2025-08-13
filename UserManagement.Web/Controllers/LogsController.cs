
using UserManagement.Data;
using System.Linq;
using UserManagement.Web.Models.Logs;
using System;

/// <summary>
/// Controller responsible for displaying application log entries.
/// NOTE: This is in-memory 
/// </summary>
public class LogsController : Controller
{
    private readonly DataContext _dataContext;

    /// <summary>
    /// Inject the EF Core data context 
    /// </summary>
    public LogsController(DataContext dataContext) => _dataContext = dataContext;

    /// <summary>
    /// Lists log entries, optionally filtered by a case-insensitive match
    /// on user forename or surname.
    /// </summary>
    /// <param name="search">Optional search text to filter user names.</param>
    public IActionResult Index(string? search)
    {
        var logsQuery =
            from log in _dataContext.LogEntries
            join user in _dataContext.Users! on log.UserId equals user.Id into userGroup
            from user in userGroup.DefaultIfEmpty() 
            orderby log.Timestamp descending
            select new LogEntryWithUserViewModel
            {
                LogId = log.Id,
                Action = log.Action,
                UserId = log.UserId,
                Forename = user != null ? user.Forename : null,
                Surname = user != null ? user.Surname : null,
                Timestamp = log.Timestamp
            };

        // Apply search filter (case-insensitive) if supplied 
        if (!string.IsNullOrWhiteSpace(search))
        {
            logsQuery = logsQuery.Where(l =>
                (l.Forename != null && l.Forename.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                (l.Surname != null && l.Surname.Contains(search, StringComparison.OrdinalIgnoreCase))
            );
        }

        //  ordered results.
        var logs = logsQuery
            .OrderByDescending(l => l.Timestamp)
            .ToList();

        return View(logs);
    }

    /// <summary>
    /// Shows the raw log entry details.
    /// </summary>
    /// <param name="id">Primary key of the log entry.</param>
    public IActionResult Details(int id)
    {
        // Fetch the log entry;
        var log = _dataContext.LogEntries.FirstOrDefault(l => l.Id == id);
        if (log == null) return NotFound();
        return View(log);
    }
}
