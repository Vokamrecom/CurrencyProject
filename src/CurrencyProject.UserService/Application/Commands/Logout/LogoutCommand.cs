using MediatR;

namespace CurrencyProject.UserService.Application.Commands.Logout;

public class LogoutCommand : IRequest<LogoutResult>
{
    public string Token { get; set; } = string.Empty;
}

public class LogoutResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
