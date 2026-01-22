using MediatR;

namespace CurrencyProject.FinanceService.Application.Queries.GetUserCurrencies;

public class GetUserCurrenciesQuery : IRequest<GetUserCurrenciesResult>
{
    public int UserId { get; set; }
}

public class CurrencyDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Rate { get; set; }
}

public class GetUserCurrenciesResult
{
    public List<CurrencyDto> Currencies { get; set; } = new();
}
