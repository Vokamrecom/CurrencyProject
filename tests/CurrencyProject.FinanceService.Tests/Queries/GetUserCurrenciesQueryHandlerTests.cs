using CurrencyProject.Domain.Entities;
using CurrencyProject.FinanceService.Application.Queries.GetUserCurrencies;
using CurrencyProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CurrencyProject.FinanceService.Tests.Queries;

public class GetUserCurrenciesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnUserFavoriteCurrencies()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);

        var currency1 = new Currency { Name = "USD", Rate = 75.50m };
        var currency2 = new Currency { Name = "EUR", Rate = 82.30m };
        var currency3 = new Currency { Name = "GBP", Rate = 95.20m };

        context.Currencies.AddRange(currency1, currency2, currency3);

        var user = new User
        {
            Name = "TestUser",
            Password = "Password",
            Favorites = new List<Currency> { currency1, currency2 }
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new GetUserCurrenciesQueryHandler(context);
        var query = new GetUserCurrenciesQuery { UserId = user.Id };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Currencies.Count);
        Assert.Contains(result.Currencies, c => c.Name == "USD");
        Assert.Contains(result.Currencies, c => c.Name == "EUR");
        Assert.DoesNotContain(result.Currencies, c => c.Name == "GBP");
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenUserHasNoFavorites()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);

        var user = new User
        {
            Name = "TestUser",
            Password = "Password",
            Favorites = new List<Currency>()
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var handler = new GetUserCurrenciesQueryHandler(context);
        var query = new GetUserCurrenciesQuery { UserId = user.Id };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Currencies);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenUserDoesNotExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);
        var handler = new GetUserCurrenciesQueryHandler(context);
        var query = new GetUserCurrenciesQuery { UserId = 999 };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Currencies);
    }
}
