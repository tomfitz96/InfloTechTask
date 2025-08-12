using System;
using System.Linq;
using FluentAssertions;
using UserManagement.Models;

namespace UserManagement.Data.Tests;

public class DataContextTests
{
    private DataContext CreateContext() => new();

    [Fact]
    public void GetAll_WhenNewEntityAdded_MustIncludeNewEntity()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var context = CreateContext();

        var entity = new User
        {
            Forename = "Brand New",
            Surname = "User",
            Email = "brandnewuser@example.com"
        };
        context.Create(entity);

        // Act: Invokes the method under test with the arranged parameters.
        var result = context.GetAll<User>();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result
            .Should().Contain(s => s.Email == entity.Email)
            .Which.Should().BeEquivalentTo(entity);
    }

    [Fact]
    public void GetAll_WhenDeleted_MustNotIncludeDeletedEntity()
    {
        // Arrange: Initializes objects and sets the value of the data that is passed to the method under test.
        var context = CreateContext();
        var entity = context.GetAll<User>().First();
        context.Delete(entity);

        // Act: Invokes the method under test with the arranged parameters.
        var result = context.GetAll<User>();

        // Assert: Verifies that the action of the method under test behaves as expected.
        result.Should().NotContain(s => s.Email == entity.Email);
    }

    [Fact]
    public void Update_WhenEntityChanged_MustReflectChanges()
    {
        // Arrange
        var context = CreateContext();
        var entity = context.GetAll<User>().First();
        entity.Forename = "Updated";
        entity.Email = "updated@example.com";

        // Act
        context.Update(entity);
        var updated = context.GetAll<User>().First(u => u.Id == entity.Id);

        // Assert
        updated.Forename.Should().Be("Updated");
        updated.Email.Should().Be("updated@example.com");
    }

    [Fact]
    public void GetAll_WhenNoEntitiesExist_ReturnsEmpty()
    {
        // Arrange
        var context = CreateContext();
        foreach (var user in context.GetAll<User>().ToList())
            context.Delete(user);

        // Act
        var result = context.GetAll<User>();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Create_WhenNullEntity_ThrowsArgumentNullException()
    {
        // Arrange
        var context = CreateContext();

        // Act
        Action act = () => context.Create<User>(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }       

    [Fact]
    public void Create_And_GetAll_LogEntry_Works()
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
        context.Create(log);
        var logs = context.GetAll<LogEntry>();

        // Assert
        logs.Should().Contain(l => l.Action == "Test" && l.Details == "Testing log entry");
    }
}
