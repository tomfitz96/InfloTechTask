using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

using Microsoft.EntityFrameworkCore;

using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;

namespace UserManagement.WebMS.Controllers;

//[Route("users")]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService) => _userService = userService;

    [HttpGet]
    public ViewResult List(bool? isActive)
    {
        IEnumerable<User> users;
        if (isActive.HasValue)
        {
            users = _userService.FilterByActive(isActive.Value);
        }
        else
        {
            users = _userService.GetAll();
        }

        
        var items = users.Select(p => new UserListItemViewModel
        {
            Id = p.Id,
            Forename = p.Forename,
            Surname = p.Surname,
            Email = p.Email,
            IsActive = p.IsActive,
            DateOfBirth = p.DateOfBirth
        });

        var model = new UserListViewModel
        {
            Items = items.ToList()
        };

        return View(model);
    }

    [HttpGet]
    // GET: Users/ViewUser
    public IActionResult ViewUser(int id)
    {
        var user = _userService.User(id);
        if (user == null)
            return NotFound();

        var model = new UserListItemViewModel
        {
            Id = user.Id,
            Forename = user.Forename,
            Surname = user.Surname,
            Email = user.Email,
            IsActive = user.IsActive,
            DateOfBirth = user.DateOfBirth
        };

        return View(model);

    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]    
    public IActionResult Create(UserListItemViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);
        
        var user = new User
        {
            Forename = model.Forename ?? string.Empty,
            Surname = model.Surname ?? string.Empty,
            Email = model.Email ?? string.Empty,
            IsActive = model.IsActive,
            DateOfBirth = model.DateOfBirth
        };

        _userService.Create(user);

        if (ModelState.IsValid)
            return RedirectToAction(nameof(List));
        else 
            return View(model);
    }
    
    // GET: Users/Edit
    [HttpGet]
    public IActionResult EditUser(int id)
    {
        var user = _userService.User(id);
        if (user == null)
        {
            return NotFound();
        }

        var model = new UserListItemViewModel
        {
            Id = user.Id,
            Forename = user.Forename,
            Surname = user.Surname,
            Email = user.Email,
            IsActive = user.IsActive,
            DateOfBirth = user.DateOfBirth
        };
        return View(model);
    }

    // POST: User/ViewUser    
    [HttpPost]    
    public IActionResult EditUser(UserListItemViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = _userService.User(model.Id);

        if (user == null)
            return NotFound();

        user.Forename = model.Forename ?? string.Empty;
        user.Surname = model.Surname ?? string.Empty;
        user.Email = model.Email ?? string.Empty;
        user.IsActive = model.IsActive;
        user.DateOfBirth = model.DateOfBirth;

        _userService.Update(user);

        if (ModelState.IsValid)
            return RedirectToAction(nameof(List));
        else 
            return View(model);
    }

    [HttpGet]
    public IActionResult DeleteUser(int id)
    {
        var user = _userService.User(id); 
        if (user == null)
            return NotFound();
     
        var model = new UserListItemViewModel
        {
            Id = user.Id,
            Forename = user.Forename,
            Surname = user.Surname,
            Email = user.Email,
            IsActive = user.IsActive,
            DateOfBirth = user.DateOfBirth
        };

        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var user = _userService.User(id);
        if (user == null)
            return NotFound();

        _userService.Delete(user); 

        return RedirectToAction(nameof(List));
    }
}
