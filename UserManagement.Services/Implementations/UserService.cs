using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Services.Domain.Implementations;

/// <summary>
/// Service providing CRUD operations for User entities plus logging of user actions.
/// </summary>
public class UserService : IUserService
{
    private readonly IDataContext _dataAccess;

    public UserService(IDataContext dataAccess) => _dataAccess = dataAccess;

    /// <summary>
    /// Returns users filtered by active status.
    /// NOTE: Currently loads all users then filters in-memory.
    /// </summary>
    public async Task<IEnumerable<User>> FilterByActiveAsync(bool isActive)
    {
        var users = await _dataAccess.GetAllAsync<User>();
        return users.Where(u => u.IsActive == isActive);
    }

    /// <summary>
    /// Retrieves all users.
    /// </summary>
    public async Task<IEnumerable<User>> GetAllAsync() => await _dataAccess.GetAllAsync<User>();

    /// <summary>
    /// Creates a new user entity.
    /// </summary>
    public async Task CreateAsync(User user)
    {
        // validation handled in view
        await _dataAccess.CreateAsync(user);
    }

    /// <summary>
    /// Retrieves a user by Id or null if not found.
    /// </summary>
    public async Task<User?> UserAsync(int id)
    {
        var users = await _dataAccess.GetAllAsync<User>();
        return users.FirstOrDefault(u => u.Id == id);
    }

    /// <summary>
    /// Updates an existing user entity.
    /// </summary>
    public async Task UpdateAsync(User existinguser)
    {
        await _dataAccess.UpdateAsync(existinguser);
    }

    /// <summary>
    /// Deletes a user entity.
    /// </summary>
    public async Task DeleteAsync(User user)
    {
        await _dataAccess.DeleteAsync(user);
    }

    /// <summary>
    /// Logs an action performed on a user.
    /// </summary>
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

    /// <summary>
    /// Returns log entries for a specific user, newest first.
    /// </summary>
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
