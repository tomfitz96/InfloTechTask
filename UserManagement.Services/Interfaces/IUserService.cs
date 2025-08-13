
using System.Collections.Generic;
//using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models;

namespace UserManagement.Services.Domain.Interfaces;

public interface IUserService 
{
    Task<IEnumerable<User>> FilterByActiveAsync(bool isActive);
    Task<IEnumerable<User>> GetAllAsync();
    Task CreateAsync(User user);
    Task<User?> UserAsync(int id);
    Task UpdateAsync(User existinguser);
    Task DeleteAsync(User user);
    Task LogUserActionAsync(int? userId, string action, string? details = null);
    Task<List<LogEntry>> GetUserLogsAsync(int userId);
}
