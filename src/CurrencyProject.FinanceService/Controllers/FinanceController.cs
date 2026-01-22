using CurrencyProject.FinanceService.Application.Queries.GetUserCurrencies;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CurrencyProject.FinanceService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FinanceController : ControllerBase
{
    private readonly IMediator _mediator;

    public FinanceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("currencies")]
    public async Task<ActionResult<GetUserCurrenciesResult>> GetUserCurrencies()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized("Неверный токен");
        }

        var query = new GetUserCurrenciesQuery { UserId = userId };
        var result = await _mediator.Send(query);

        return Ok(result);
    }
}
