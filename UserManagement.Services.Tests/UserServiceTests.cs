using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models;
using UserManagement.Services.Domain.Implementations;

namespace UserManagement.Data.Tests;

public class UserServiceTests
{
    private readonly Mock<IDataContext> _dataContext = new();
    private UserService CreateService() => new(_dataContext.Object);
    private Task<IQueryable<User>> SetupUsers(string forename = "Johnny", string surname = "User", string email = "juser@example.com", bool isActive = true)
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
            .Setup(s => s.GetAllAsync<User>())
            .ReturnsAsync(users.ToList());

        return Task.FromResult(users);
    }

    [Fact]
    public async Task GetAll_WhenContextReturnsEntities_MustReturnSameEntities()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var service = CreateService();
        var users = await SetupUsers();

        // Act: Invokes the method under test with the arranged parameters.
        var result = await service.GetAllAsync();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().BeEquivalentTo(users);
    }

    [Fact]
    public async Task FilterByActive_WhenCalled_ReturnsOnlyMatchingUsers()
    {
        // Arrange
        var service = CreateService();
        var users = new[]
        {
            new User { Forename = "Active", Surname = "User", Email = "active@example.com", IsActive = true },
            new User { Forename = "Inactive", Surname = "User", Email = "inactive@example.com", IsActive = false }
        }.AsQueryable();

        _dataContext.Setup(s => s.GetAllAsync<User>()).ReturnsAsync(users.ToList());

        // Act
        var activeUsers = (await service.FilterByActiveAsync(true)).ToList();
        var inactiveUsers = (await service.FilterByActiveAsync(false)).ToList();

        // Assert
        activeUsers.Should().ContainSingle(u => u.IsActive);
        inactiveUsers.Should().ContainSingle(u => !u.IsActive);
    }

    [Fact]
    public async Task Create_CallsDataContextCreate()
    {
        // Arrange
        var service = CreateService();
        var user = new User { Forename = "Test", Surname = "User", Email = "test@example.com", IsActive = true };

        // Act
        await service.CreateAsync(user);

        // Assert
        _dataContext.Verify(c => c.CreateAsync(user), Times.Once);
    }

    [Fact]
    public async Task User_WhenIdExists_ReturnsUser()
    {
        // Arrange
        var service = CreateService();
        var users = new[]
        {
        new User { Id = 1, Forename = "A", Surname = "B", Email = "a@b.com", IsActive = true }}.AsQueryable();

        _dataContext.Setup(s => s.GetAllAsync<User>()).ReturnsAsync(users.ToList());

        // Act
        var result = await service.UserAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
    }

    [Fact]
    public async Task User_WhenIdDoesNotExist_ReturnsNull()
    {
        // Arrange
        var service = CreateService();
        var users = new User[0].AsQueryable();
        _dataContext.Setup(s => s.GetAllAsync<User>()).ReturnsAsync(users.ToList());

        // Act
        var result = await service.UserAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Update_CallsDataContextUpdate()
    {
        // Arrange
        var service = CreateService();
        var user = new User { Id = 1, Forename = "A", Surname = "B", Email = "a@b.com", IsActive = true };

        // Act
        await service.UpdateAsync(user);

        // Assert
        _dataContext.Verify(c => c.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task Delete_CallsDataContextDelete()
    {
        // Arrange
        var service = CreateService();
        var user = new User { Id = 1, Forename = "A", Surname = "B", Email = "a@b.com", IsActive = true };

        // Act
        await service.DeleteAsync(user);

        // Assert
        _dataContext.Verify(c => c.DeleteAsync(user), Times.Once);
    }
}
