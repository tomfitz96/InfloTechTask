//using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models;

namespace UserManagement.Data;

public interface IDataContext
{
    Task<List<TEntity>> GetAllAsync<TEntity>() where TEntity : class;
    Task CreateAsync<TEntity>(TEntity entity) where TEntity : class;
    Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class;
    Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class;
    IQueryable<LogEntry> LogEntries { get; }
}
