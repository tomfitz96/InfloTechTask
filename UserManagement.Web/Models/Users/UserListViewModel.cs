using System;
using System.ComponentModel.DataAnnotations;

namespace UserManagement.Web.Models.Users;

public class UserListViewModel
{
    public List<UserListItemViewModel> Items { get; set; } = new();
}

public class UserListItemViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Forename is required.")]
    [StringLength(50, ErrorMessage = "Forename cannot be longer than 50 characters.")]
    public string? Forename { get; set; }

    [Required(ErrorMessage = "Surname is required.")]
    [StringLength(50, ErrorMessage = "Surname cannot be longer than 50 characters.")]
    public string? Surname { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string? Email { get; set; }

    public bool IsActive { get; set; }

    [Required(ErrorMessage = "Date of Birth is required.")]
    public DateOnly DateOfBirth { get; set; }
}
