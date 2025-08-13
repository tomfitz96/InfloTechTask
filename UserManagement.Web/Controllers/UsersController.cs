using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;

namespace UserManagement.WebMS.Controllers;
public class UsersController : Controller
{
    private readonly IUserService _userService;

    /// <summary>
    /// Injects the user service dependency.
    /// </summary>
    public UsersController(IUserService userService) => _userService = userService;

    /// <summary>
    /// Lists users, filtered by active status.
    /// </summary>
    [HttpGet]
    public async Task<ViewResult> List(bool? isActive)
    {
        IEnumerable<User> users = isActive.HasValue
            ? await _userService.FilterByActiveAsync(isActive.Value)
            : await _userService.GetAllAsync();

        // map to view models.
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

    /// <summary>
    /// Displays a single user and their logs.
    /// </summary>
    /// <param name="id">User identifier.</param>
    [HttpGet]
    public async Task<IActionResult> ViewUser(int id)
    {
        // Fetch user;
        var user = await _userService.UserAsync(id);
        if (user == null)
            return NotFound();

        // Retrieve associated logs.
        var logs = await _userService.GetUserLogsAsync(id);

        // Map to view model including logs.
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

    /// <summary>
    /// Shows the create user form.
    /// </summary>
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    /// <summary>
    /// Handles creation of a new user.
    /// </summary>
    /// <param name="model">User input model.</param>
    [HttpPost]
    public async Task<IActionResult> Create(UserListItemViewModel model)
    {
        // Return with validation errors if model invalid.
        if (!ModelState.IsValid)
            return View(model);

        // Map to model.
        var user = new User
        {
            Forename = model.Forename ?? string.Empty,
            Surname = model.Surname ?? string.Empty,
            Email = model.Email ?? string.Empty,
            IsActive = model.IsActive,
            DateOfBirth = model.DateOfBirth
        };

        // Persist and log
        await _userService.CreateAsync(user);
        await _userService.LogUserActionAsync(user.Id, "Created", $"User {user.Forename} {user.Surname} created.");

        // redirect back to list view
        return RedirectToAction(nameof(List));
    }

    /// <summary>
    /// Displays the edit form for an existing user.
    /// </summary>
    /// <param name="id">User identifier.</param>
    [HttpGet]
    public async Task<IActionResult> EditUser(int id)
    {
        var user = await _userService.UserAsync(id);
        if (user == null)
            return NotFound();

        // Populate form model.
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

    /// <summary>
    /// Handles user update submission.
    /// </summary>
    /// <param name="model">Updated user data.</param>
    [HttpPost]
    public async Task<IActionResult> EditUser(UserListItemViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        // Fetch current entity.
        var user = await _userService.UserAsync(model.Id);
        if (user == null)
            return NotFound();

        // add changes to log
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

        // Apply updates.
        user.Forename = model.Forename ?? string.Empty;
        user.Surname = model.Surname ?? string.Empty;
        user.Email = model.Email ?? string.Empty;
        user.IsActive = model.IsActive;
        user.DateOfBirth = model.DateOfBirth;

        // Persist only if there are actual changes.
        if (changes.Count > 0)
        {
            await _userService.UpdateAsync(user);
            await _userService.LogUserActionAsync(user.Id, "Updated", string.Join("; ", changes));
        }

        return RedirectToAction(nameof(ViewUser), new { id = user.Id });
    }

    /// <summary>
    /// Shows delete confirmation page.
    /// </summary>
    /// <param name="id">User identifier.</param>
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

    /// <summary>
    /// Performs the deletion after confirmation.
    /// </summary>
    /// <param name="id">User identifier.</param>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var user = await _userService.UserAsync(id);
        if (user == null)
            return NotFound();

        // Delete and log.
        await _userService.DeleteAsync(user);
        await _userService.LogUserActionAsync(user.Id, "Deleted", $"User {user.Forename} {user.Surname} deleted.");
        return RedirectToAction(nameof(List));
    }
}
