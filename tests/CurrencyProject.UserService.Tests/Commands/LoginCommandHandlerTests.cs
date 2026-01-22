using CurrencyProject.Domain.Entities;
using CurrencyProject.Infrastructure.Data;
using CurrencyProject.UserService.Application.Commands.Login;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace CurrencyProject.UserService.Tests.Commands;

public class LoginCommandHandlerTests
{
    private readonly IConfiguration _configuration;

    public LoginCommandHandlerTests()
    {
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "Jwt:Key", "YourSuperSecretKeyThatIsAtLeast32CharactersLong!" },
            { "Jwt:Issuer", "CurrencyProject" },
            { "Jwt:Audience", "CurrencyProject" }
        });
        _configuration = configBuilder.Build();
    }

    [Fact]
    public async Task Handle_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);
        var handler = new LoginCommandHandler(context, _configuration);

        var user = new User
        {
            Name = "TestUser",
            Password = BCrypt.Net.BCrypt.HashPassword("TestPassword123")
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var command = new LoginCommand
        {
            Name = "TestUser",
            Password = "TestPassword123"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotEmpty(result.Token);
        Assert.Equal("Успешный вход", result.Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenUserDoesNotExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);
        var handler = new LoginCommandHandler(context, _configuration);

        var command = new LoginCommand
        {
            Name = "NonExistentUser",
            Password = "Password"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Неверное имя пользователя или пароль", result.Message);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenPasswordIsIncorrect()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);
        var handler = new LoginCommandHandler(context, _configuration);

        var user = new User
        {
            Name = "TestUser",
            Password = BCrypt.Net.BCrypt.HashPassword("CorrectPassword")
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var command = new LoginCommand
        {
            Name = "TestUser",
            Password = "WrongPassword"
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Неверное имя пользователя или пароль", result.Message);
    }
}
