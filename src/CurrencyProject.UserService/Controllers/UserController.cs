using CurrencyProject.UserService.Application.Commands.Login;
using CurrencyProject.UserService.Application.Commands.Logout;
using CurrencyProject.UserService.Application.Commands.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyProject.UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterUserResult>> Register([FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResult>> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (!result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<ActionResult<LogoutResult>> Logout([FromBody] LogoutCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
