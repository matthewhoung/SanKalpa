using Microsoft.VisualBasic;
using SanKalpa.Application.Abstrations.Authentication;
using SanKalpa.Application.Abstrations.Messaging;
using SanKalpa.Domain.Abstrations;
using SanKalpa.Domain.Services;
using SanKalpa.Domain.Users;

namespace SanKalpa.Application.Users.Login;

internal sealed class LoginCommandHandler : ICommandHandler<LoginCommand, string>
{
    private readonly IJwtService _jwtService;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHashService _passwordHashService;

    private static readonly Error InvalidEmail = new(
        "404 Bad Request",
        "LoginHandler : Invalid EmailAddress.");

    private static readonly Error InvalidPassword = new(
        "404 Bad Request",
        "LoginHandler : Invalid Password.");

    public LoginCommandHandler(
        IJwtService jwtService,
        IUserRepository userRepository,
        IPasswordHashService passwordHashService)
    {
        _jwtService = jwtService;
        _userRepository = userRepository;
        _passwordHashService = passwordHashService;
    }

    public async Task<Result<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        User user = await _userRepository.GetByEmailAsync(request.EmailAddress, cancellationToken);
        if (user == null)
        {
            return Result.Failure<string>(InvalidEmail);
        }

        bool isValidPassword = _passwordHashService.VerifyHashedPassword(
            user.Password.Value,
            request.Password);

        if (!isValidPassword)
        {
            return Result.Failure<string>(InvalidPassword);
        }

        var token = _jwtService.TokenGenerator(
            user.Id,
            user.UserName.Value,
            user.EmailAddress.Value,
            user.Password.Value);

        return Result.Success(token.Value);

    }
}
