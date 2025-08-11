
using System.Collections.Generic;
using System.Linq;
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
    public IEnumerable<User> FilterByActive(bool isActive)
    {
        return _dataAccess.GetAll<User>().Where(u => u.IsActive == isActive);
    }

    public IEnumerable<User> GetAll() => _dataAccess.GetAll<User>();

    public void Create(User user)
    {        
        _dataAccess.Create(user);
    }

    public User? User(int id)
    {

        return _dataAccess.GetAll<User>().Where(u => u.Id == id).FirstOrDefault();
    }

    public void Update(User existinguser)
    {
        _dataAccess.Update(existinguser);
    }

    public void Delete(User user)
    {
        _dataAccess.Delete(user);
     
    }
}
