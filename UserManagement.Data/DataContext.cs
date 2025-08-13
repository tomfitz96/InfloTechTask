using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;

namespace UserManagement.Data;

/// <summary>
/// Application EF Core DbContext backed by an in-memory database.
/// NOTE: Data is "In-memory" which is suitable for demos/tests, should be replaced with a persistent provider (e.g. SQL Server)
/// </summary>
public class DataContext : DbContext, IDataContext
{
    /// <summary>
    /// Constructor ensures the in-memory store is created.    
    /// </summary>
    public DataContext() => Database.EnsureCreated();

    /// <summary>
    /// Configure EF Core to use the in-memory database.
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseInMemoryDatabase("UserManagement.Data.DataContext");

    protected override void OnModelCreating(ModelBuilder model)
        => model.Entity<User>().HasData(new[]
        {
            new User { Id = 1, Forename = "Peter", Surname = "Loew", Email = "ploew@example.com", IsActive = true, DateOfBirth = new DateOnly(1953, 7, 10)},
            new User { Id = 2, Forename = "Benjamin Franklin", Surname = "Gates", Email = "bfgates@example.com", IsActive = true, DateOfBirth = new DateOnly(1964, 5, 17)},
            new User { Id = 3, Forename = "Castor", Surname = "Troy", Email = "ctroy@example.com", IsActive = false, DateOfBirth = new DateOnly(1965, 4, 2)},
            new User { Id = 4, Forename = "Memphis", Surname = "Raines", Email = "mraines@example.com", IsActive = true, DateOfBirth = new DateOnly(1968,11,22)},
            new User { Id = 5, Forename = "Stanley", Surname = "Goodspeed", Email = "sgodspeed@example.com", IsActive = true , DateOfBirth = new DateOnly(1969, 8, 14)},
            new User { Id = 6, Forename = "H.I.", Surname = "McDunnough", Email = "himcdunnough@example.com", IsActive = true , DateOfBirth = new DateOnly(1959,10,3)},
            new User { Id = 7, Forename = "Cameron", Surname = "Poe", Email = "cpoe@example.com", IsActive = false , DateOfBirth = new DateOnly(1954,1,21)},
            new User { Id = 8, Forename = "Edward", Surname = "Malus", Email = "emalus@example.com", IsActive = false , DateOfBirth = new DateOnly(1966,3,27)},
            new User { Id = 9, Forename = "Damon", Surname = "Macready", Email = "dmacready@example.com", IsActive = false , DateOfBirth = new DateOnly(1967,12,12)},
            new User { Id = 10, Forename = "Johnny", Surname = "Blaze", Email = "jblaze@example.com", IsActive = true , DateOfBirth = new DateOnly(1966,6,6)},
            new User { Id = 11, Forename = "Robin", Surname = "Feld", Email = "rfeld@example.com", IsActive = true , DateOfBirth = new DateOnly(1957,2,5)},
        });

    /// <summary>
    /// Users aggregate root set.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Backing DbSet for log entries (getter-only to prevent reassignment).
    /// </summary>
    public DbSet<LogEntry> LogEntriesDbSet => Set<LogEntry>();

    /// <summary>
    /// Read-only projection of log entries
    /// </summary>
    public IQueryable<LogEntry> LogEntries => LogEntriesDbSet;

    /// <summary>
    /// Retrieve all entities of a given type.
    /// NOTE: This loads all rows into memory; 
    /// </summary>
    public async Task<List<TEntity>> GetAllAsync<TEntity>() where TEntity : class
        => await Set<TEntity>().ToListAsync();

    /// <summary>
    /// Create (add) an entity and persist immediately.
    /// </summary>
    public async Task CreateAsync<TEntity>(TEntity entity) where TEntity : class
    {
        await AddAsync(entity);
        await SaveChangesAsync();
    }

    /// <summary>
    /// Update an entity and persist immediately.
    /// Assumes the entity is either tracked or can be attached safely.
    /// </summary>
    public async Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class
    {
        Update(entity);
        await SaveChangesAsync();
    }

    /// <summary>
    /// Delete an entity and persist immediately.
    /// </summary>
    public async Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class
    {
        Remove(entity);
        await SaveChangesAsync();
    }
}
