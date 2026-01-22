using MediatR;

namespace CurrencyProject.UserService.Application.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, LogoutResult>
{
    public Task<LogoutResult> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        // В случае JWT токенов, логаут обычно обрабатывается на клиенте
        // путем удаления токена. Здесь просто возвращаем успех.
        return Task.FromResult(new LogoutResult
        {
            Success = true,
            Message = "Успешный выход"
        });
    }
}
