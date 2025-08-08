using System.Linq;
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

        //var items = _userService.GetAll().Select(p => new UserListItemViewModel
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
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(UserListItemViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        // In the Create(UserListItemViewModel model) method, update the mapping to handle possible nulls
        var user = new User
        {
            Forename = model.Forename ?? string.Empty,
            Surname = model.Surname ?? string.Empty,
            Email = model.Email ?? string.Empty,
            IsActive = model.IsActive,
            DateOfBirth = model.DateOfBirth
        };

        _userService.Create(user); 

        return RedirectToAction(nameof(List));
    }

}
