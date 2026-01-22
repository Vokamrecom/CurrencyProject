using MediatR;

namespace CurrencyProject.UserService.Application.Commands.RegisterUser;

public class RegisterUserCommand : IRequest<RegisterUserResult>
{
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterUserResult
{
    public int UserId { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
