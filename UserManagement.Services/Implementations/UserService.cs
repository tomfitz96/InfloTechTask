using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Services.Domain.Implementations;

public class UserService : IUserService
{
    private readonly IDataContext _dataAccess;
    public UserService(IDataContext dataAccess) => _dataAccess = dataAccess;

    /// <summary>
    /// Return users by active state
    /// </summary>
    /// <param name="isActive"></param>
    /// <returns></returns>
    

    public async Task<IEnumerable<User>> FilterByActiveAsync(bool isActive)
    {
        var users = await _dataAccess.GetAllAsync<User>();
        return users.Where(u => u.IsActive == isActive);
    }
    public async Task<IEnumerable<User>> GetAllAsync() => await _dataAccess.GetAllAsync<User>();

    public async Task CreateAsync(User user)
    {
        await _dataAccess.CreateAsync(user);
    }

    public async Task<User?> UserAsync(int id)
    {
        var users = await _dataAccess.GetAllAsync<User>();
        return users.Where(u => u.Id == id).FirstOrDefault();        
    }

    public async Task UpdateAsync(User existinguser)
    {
        await _dataAccess.UpdateAsync(existinguser);
    }

    public async Task DeleteAsync(User user)
    {
        await _dataAccess.DeleteAsync(user);     
    }

    public async Task LogUserActionAsync(int? userId, string action, string? details = null)
    {
        var log = new LogEntry
        {
            UserId = userId,
            Action = action,
            Details = details,
            Timestamp = DateTime.UtcNow
        };
        await _dataAccess.CreateAsync(log);
    }

    public async Task<List<LogEntry>> GetUserLogsAsync(int userId)
    {        
        return await Task.FromResult(
            _dataAccess.LogEntries
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.Timestamp)
                .ToList()
        );
    }
}
