
using UserManagement.Data;
using System.Linq;
using UserManagement.Web.Models.Logs;
using System;

public class LogsController : Controller
{
    private readonly DataContext _dataContext;
    public LogsController(DataContext dataContext) => _dataContext = dataContext;

    public IActionResult Index(string? search)
    {
        var logsQuery = (from log in _dataContext.LogEntries
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
                         });


        if(!string.IsNullOrWhiteSpace(search))
        {
            logsQuery = logsQuery.Where(l =>
            (l.Forename != null && l.Forename.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
            (l.Surname != null && l.Surname.Contains(search, StringComparison.OrdinalIgnoreCase))
            );
        }

        var logs = logsQuery.OrderByDescending(l => l.Timestamp).ToList();

        return View(logs);
    }

  
    public IActionResult Details(int id)
    {
        var log = _dataContext.LogEntries.FirstOrDefault(l => l.Id == id);
        if (log == null) return NotFound();
        return View(log);
    }
}
