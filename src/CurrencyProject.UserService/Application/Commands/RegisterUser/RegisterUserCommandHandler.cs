using CurrencyProject.Domain.Entities;
using CurrencyProject.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CurrencyProject.UserService.Application.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly ApplicationDbContext _context;

    public RegisterUserCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RegisterUserResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Name == request.Name, cancellationToken);

        if (existingUser != null)
        {
            return new RegisterUserResult
            {
                Success = false,
                Message = "Пользователь с таким именем уже существует"
            };
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Name = request.Name,
            Password = hashedPassword
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return new RegisterUserResult
        {
            Success = true,
            UserId = user.Id,
            Message = "Пользователь успешно зарегистрирован"
        };
    }
}
