using CurrencyProject.Infrastructure.Data;
using CurrencyProject.UserService.Application.Commands.RegisterUser;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CurrencyProject.UserService.Tests.Commands;

public class RegisterUserCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldRegisterNewUser_WhenUserDoesNotExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);
        var handler = new RegisterUserCommandHandler(context);

        var command = new RegisterUserCommand
        {
            Name = "TestUser",
            Password = "TestPassword123"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.True(result.UserId > 0);
        Assert.Equal("Пользователь успешно зарегистрирован", result.Message);

        var user = await context.Users.FirstOrDefaultAsync(u => u.Name == "TestUser");
        Assert.NotNull(user);
        Assert.True(BCrypt.Net.BCrypt.Verify("TestPassword123", user.Password));
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserAlreadyExists()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);
        var handler = new RegisterUserCommandHandler(context);

        var existingUser = new CurrencyProject.Domain.Entities.User
        {
            Name = "ExistingUser",
            Password = BCrypt.Net.BCrypt.HashPassword("Password")
        };
        context.Users.Add(existingUser);
        await context.SaveChangesAsync();

        var command = new RegisterUserCommand
        {
            Name = "ExistingUser",
            Password = "NewPassword"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Пользователь с таким именем уже существует", result.Message);
    }
}
