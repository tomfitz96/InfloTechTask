using System.Linq;
using UserManagement.Models;
using UserManagement.Services.Domain.Implementations;

namespace UserManagement.Data.Tests;

public class UserServiceTests
{
    private readonly Mock<IDataContext> _dataContext = new();
    private UserService CreateService() => new(_dataContext.Object);
    private IQueryable<User> SetupUsers(string forename = "Johnny", string surname = "User", string email = "juser@example.com", bool isActive = true)
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
        }.AsQueryable();

        _dataContext
            .Setup(s => s.GetAll<User>())
            .Returns(users);

        return users;
    }

    [Fact]
    public void GetAll_WhenContextReturnsEntities_MustReturnSameEntities()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var service = CreateService();
        var users = SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = service.GetAll();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeSameAs(users);
    }

    [Fact]
    public void FilterByActive_WhenCalled_ReturnsOnlyMatchingUsers()
    {
        // Arrange
        var service = CreateService();
        var users = new[]
        {
        new User { Forename = "Active", Surname = "User", Email = "active@example.com", IsActive = true },
        new User { Forename = "Inactive", Surname = "User", Email = "inactive@example.com", IsActive = false }
    }.AsQueryable();

        _dataContext.Setup(s => s.GetAll<User>()).Returns(users);

        // Act
        var activeUsers = service.FilterByActive(true).ToList();
        var inactiveUsers = service.FilterByActive(false).ToList();

        // Assert
        activeUsers.Should().ContainSingle(u => u.IsActive);
        inactiveUsers.Should().ContainSingle(u => !u.IsActive);
    }

    [Fact]
    public void Create_CallsDataContextCreate()
    {
        // Arrange
        var service = CreateService();
        var user = new User { Forename = "Test", Surname = "User", Email = "test@example.com", IsActive = true };

        // Act
        service.Create(user);

        // Assert
        _dataContext.Verify(c => c.Create(user), Times.Once);
    }

    [Fact]
    public void User_WhenIdExists_ReturnsUser()
    {
        // Arrange
        var service = CreateService();
        var users = new[]
        {
        new User { Id = 1, Forename = "A", Surname = "B", Email = "a@b.com", IsActive = true }}.AsQueryable();

        _dataContext.Setup(s => s.GetAll<User>()).Returns(users);

        // Act
        var result = service.User(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public void User_WhenIdDoesNotExist_ReturnsNull()
    {
        // Arrange
        var service = CreateService();
        var users = new User[0].AsQueryable();
        _dataContext.Setup(s => s.GetAll<User>()).Returns(users);

        // Act
        var result = service.User(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Update_CallsDataContextUpdate()
    {
        // Arrange
        var service = CreateService();
        var user = new User { Id = 1, Forename = "A", Surname = "B", Email = "a@b.com", IsActive = true };

        // Act
        service.Update(user);

        // Assert
        _dataContext.Verify(c => c.Update(user), Times.Once);
    }

    [Fact]
    public void Delete_CallsDataContextDelete()
    {
        // Arrange
        var service = CreateService();
        var user = new User { Id = 1, Forename = "A", Surname = "B", Email = "a@b.com", IsActive = true };

        // Act
        service.Delete(user);

        // Assert
        _dataContext.Verify(c => c.Delete(user), Times.Once);
    }
}
