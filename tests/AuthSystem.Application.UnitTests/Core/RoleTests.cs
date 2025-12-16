using AuthSystem.Core.Entities;
using FluentAssertions;

namespace AuthSystem.Application.UnitTests.Core;

public class RoleTests
{
    [Fact]
    public void Role_AdminConstant_ShouldBeAdmin()
    {
        // Act & Assert
        Role.Admin.Should().Be("Admin");
    }

    [Fact]
    public void Role_UserConstant_ShouldBeUser()
    {
        // Act & Assert
        Role.User.Should().Be("User");
    }

    [Fact]
    public void Role_Constants_ShouldNotBeNull()
    {
        // Act & Assert
        Role.Admin.Should().NotBeNull();
        Role.User.Should().NotBeNull();
    }

    [Fact]
    public void Role_Constants_ShouldNotBeEmpty()
    {
        // Act & Assert
        Role.Admin.Should().NotBeEmpty();
        Role.User.Should().NotBeEmpty();
    }
}