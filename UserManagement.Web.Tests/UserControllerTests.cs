using System;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;
using UserManagement.WebMS.Controllers;

namespace UserManagement.Data.Tests;

public class UserControllerTests
{
    [Fact]
    public void List_WhenServiceReturnsUsers_ModelMustContainUsers()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var controller = CreateController();
        var users = SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = controller.List(null);

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Model
            .Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().BeEquivalentTo(users);
    }

    [Fact]
    public void List_WhenIsActiveProvided_FiltersUsersCorrectly()
    {
        // Arrange: 
        var controller = CreateController();
        var activeUsers = new[]
        {
        new User { Forename = "Active", Surname = "User", Email = "active@example.com", IsActive = true }};
        _userService.Setup(s => s.FilterByActive(true)).Returns(activeUsers);

        // Act
        var result = controller.List(true);

        // Assert
        result.Model.Should().BeOfType<UserListViewModel>()
            .Which.Items.Should().BeEquivalentTo(activeUsers);
    }

    [Fact]
    public void Create_Get_ReturnsViewResult()
    {
        // Arrange
        var controller = CreateController();

        // Act
        var result = controller.Create();

        // Assert
        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public void Create_Post_WithInvalidModel_ReturnsViewWithModel()
    {
        // Arrange
        var controller = CreateController();
        controller.ModelState.AddModelError("Forename", "Required");
        var model = new UserListItemViewModel();

        // Act
        var result = controller.Create(model);

        // Assert
        result.Should().BeOfType<ViewResult>()
            .Which.Model.Should().Be(model);
    }

    [Fact]
    public void Create_Post_WithValidModel_CreatesUserAndRedirects()
    {
        // Arrange
        var controller = CreateController();
        var model = new UserListItemViewModel
        {
            Forename = "Test",
            Surname = "User",
            Email = "test@example.com",
            IsActive = true,
            DateOfBirth = DateOnly.FromDateTime(DateTime.Today)
        };

        // Act
        var result = controller.Create(model);

        // Assert
        _userService.Verify(s => s.Create(It.IsAny<User>()), Times.Once);
        result.Should().BeOfType<RedirectToActionResult>();
    }

    [Fact]
    public void ViewUser_WhenUserNotFound_ReturnsNotFound()
    {
        // Arrange
        var controller = CreateController();
        _userService.Setup(s => s.User(It.IsAny<int>())).Returns((User?)null);

        // Act
        var result = controller.ViewUser(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void EditUser_Get_WhenUserNotFound_ReturnsNotFound()
    {
        // Arrange
        var controller = CreateController();
        _userService.Setup(s => s.User(It.IsAny<int>())).Returns((User?)null);

        // Act
        var result = controller.EditUser(999);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    private User[] SetupUsers(string forename = "Johnny", string surname = "User", string email = "juser@example.com", bool isActive = true)
    {
        var users = new[]
        {
            new User
            {
                Forename = forename,
                Surname = surname,
                Email = email,
                IsActive = isActive
            }
        };

        _userService
            .Setup(s => s.GetAll())
            .Returns(users);

        return users;
    }

    private readonly Mock<IUserService> _userService = new();
    private readonly Mock<IDataContext> _dataContext = new();

    private UsersController CreateController() => new(_userService.Object, _dataContext.Object);
}
