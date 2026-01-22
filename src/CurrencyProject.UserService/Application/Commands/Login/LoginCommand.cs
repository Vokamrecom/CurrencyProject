using MediatR;

namespace CurrencyProject.UserService.Application.Commands.Login;

public class LoginCommand : IRequest<LoginResult>
{
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResult
{
    public bool Success { get; set; }
    public string Token { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
