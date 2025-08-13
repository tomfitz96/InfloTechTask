using System.Linq;
using System.Threading.Tasks;
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
    public async Task<ViewResult> List(bool? isActive)
    {
        IEnumerable<User> users;
        if (isActive.HasValue)
        {
            users = await _userService.FilterByActiveAsync(isActive.Value);
        }
        else
        {
            users = await _userService.GetAllAsync();
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
    public async Task<IActionResult> ViewUser(int id)
    {
        var user = await _userService.UserAsync(id);
        if (user == null)
            return NotFound();

        var logs = await _userService.GetUserLogsAsync(id);
        var model = new UserListItemViewModel
        {

            Id = user.Id,
            Forename = user.Forename,
            Surname = user.Surname,
            Email = user.Email,
            IsActive = user.IsActive,
            DateOfBirth = user.DateOfBirth,
            Logs = logs
        };

        return View(model);

    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]    
    public async Task<IActionResult> Create(UserListItemViewModel model)
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

        await _userService.CreateAsync(user);
        await _userService.LogUserActionAsync(user.Id, "Created", $"User {user.Forename} {user.Surname} created.");

        if (ModelState.IsValid)
            return RedirectToAction(nameof(List));
        else 
            return View(model);
    }
    
    // GET: Users/Edit
    [HttpGet]
    public async Task<IActionResult> EditUser(int id)
    {
        var user = await _userService.UserAsync(id);
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
    public async Task<IActionResult> EditUser(UserListItemViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userService.UserAsync(model.Id);

        if (user == null)
            return NotFound();

        

        var changes = new List<string>();

        if (user.Forename != model.Forename)
            changes.Add($"Forename changed from {user.Forename} to {model.Forename}");
        if (user.Surname != model.Surname)
            changes.Add($"Surname changed from {user.Surname} to {model.Surname}");
        if (user.Email != model.Email)
            changes.Add($"Email changed from {user.Email} to {model.Email}");
        if (user.IsActive != model.IsActive)
            changes.Add($"IsActive changed from {user.IsActive} to {model.IsActive}");
        if (user.DateOfBirth != model.DateOfBirth)
            changes.Add($"DateOfBirth changed from {user.DateOfBirth} to {model.DateOfBirth}");

        user.Forename = model.Forename ?? string.Empty;
        user.Surname = model.Surname ?? string.Empty;
        user.Email = model.Email ?? string.Empty;
        user.IsActive = model.IsActive;
        user.DateOfBirth = model.DateOfBirth;

        if (changes.Count > 0)
        {
            await _userService.UpdateAsync(user);
            await _userService.LogUserActionAsync(user.Id, "Updated", string.Join("; ",changes));
        }
       
        if (ModelState.IsValid)
            return RedirectToAction(nameof(ViewUser), new { id = user.Id });       
        else 
            return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _userService.UserAsync(id); 
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
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var user = await _userService.UserAsync(id);
        if (user == null)
            return NotFound();

        await _userService.DeleteAsync(user);
        await _userService.LogUserActionAsync(user.Id, "Deleted", $"User {user.Forename} {user.Surname} deleted.");
        return RedirectToAction(nameof(List));
    }
}
