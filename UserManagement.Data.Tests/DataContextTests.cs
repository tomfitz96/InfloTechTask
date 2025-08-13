using System;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Models;

namespace UserManagement.Data.Tests;

public class DataContextTests
{
    private DataContext CreateContext() => new();

    [Fact]
    public async Task GetAll_WhenNewEntityAdded_MustIncludeNewEntity()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var context = CreateContext();

        var entity = new User
        {
            Forename = "Brand New",
            Surname = "User",
            Email = "brandnewuser@example.com"
        };
        await context.CreateAsync(entity);

        // Act: Invokes the method under test with the arranged parameters.
        var result = await context.GetAllAsync<User>();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result
            .Should().Contain(s => s.Email == entity.Email)
            .Which.Should().BeEquivalentTo(entity);
    }

    [Fact]
    public async Task GetAll_WhenDeleted_MustNotIncludeDeletedEntity()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var context = CreateContext();
        var users = await context.GetAllAsync<User>();
        var entity = users.First();
        await context.DeleteAsync(entity);

        // Act: Invokes the method under test with the arranged parameters.
        var result = await context.GetAllAsync<User>();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().NotContain(s => s.Email == entity.Email);
    }

    [Fact]
    public async Task Update_WhenEntityChanged_MustReflectChanges()
    {
        // Arrange
        var context = CreateContext();
        var entity = (await context.GetAllAsync<User>()).First();
        entity.Forename = "Updated";
        entity.Email = "updated@example.com";

        // Act
        await context.UpdateAsync(entity);
        var updated = (await context.GetAllAsync<User>()).First(u => u.Id == entity.Id);

        // Assert
        updated.Forename.Should().Be("Updated");
        updated.Email.Should().Be("updated@example.com");
    }

    [Fact]
    public async Task GetAll_WhenNoEntitiesExist_ReturnsEmpty()
    {
        // Arrange
        var context = CreateContext();
        var users = await context.GetAllAsync<User>();
        foreach (var user in users)
            await context.DeleteAsync(user);

        // Act
        var result = await context.GetAllAsync<User>();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Create_WhenNullEntity_ThrowsArgumentNullException()
    {
        // Arrange
        var context = CreateContext();

        // Act
        Func<Task> act = async () => await context.CreateAsync<User>(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task Create_And_GetAll_LogEntry_Works()
    {
        // Arrange
        var context = CreateContext();
        var log = new LogEntry
        {
            Action = "Test",
            Details = "Testing log entry",
            Timestamp = DateTime.UtcNow
        };

        // Act
        await context.CreateAsync(log);
        var logs = await context.GetAllAsync<LogEntry>();

        // Assert
        logs.Should().Contain(l => l.Action == "Test" && l.Details == "Testing log entry");
    }
}
