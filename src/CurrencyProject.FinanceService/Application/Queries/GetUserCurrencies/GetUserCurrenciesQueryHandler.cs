using CurrencyProject.FinanceService.Application.Queries.GetUserCurrencies;
using CurrencyProject.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CurrencyProject.FinanceService.Application.Queries.GetUserCurrencies;

public class GetUserCurrenciesQueryHandler : IRequestHandler<GetUserCurrenciesQuery, GetUserCurrenciesResult>
{
    private readonly ApplicationDbContext _context;

    public GetUserCurrenciesQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<GetUserCurrenciesResult> Handle(GetUserCurrenciesQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.Favorites)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            return new GetUserCurrenciesResult();
        }

        var currencies = user.Favorites.Select(c => new CurrencyDto
        {
            Id = c.Id,
            Name = c.Name,
            Rate = c.Rate
        }).ToList();

        return new GetUserCurrenciesResult
        {
            Currencies = currencies
        };
    }
}
