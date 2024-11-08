using SanKalpa.Application.Abstrations.Authentication;
using SanKalpa.Application.Abstrations.Messaging;
using SanKalpa.Domain.Abstrations;
using SanKalpa.Domain.Services;
using SanKalpa.Domain.Users;

namespace SanKalpa.Application.Users.Login;

internal sealed class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResponse>
{
    private readonly IJwtService _jwtService;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IRefreshTokenService _refreshTokenService;

    private static readonly Error InvalidEmail = new(
        "404 Bad Request",
        "LoginHandler : Invalid EmailAddress.");

    private static readonly Error InvalidPassword = new(
        "404 Bad Request",
        "LoginHandler : Invalid Password.");

    public LoginCommandHandler(
        IJwtService jwtService,
        IUserRepository userRepository,
        IPasswordHashService passwordHashService,
        IRefreshTokenService refreshTokenService)
    {
        _jwtService = jwtService;
        _userRepository = userRepository;
        _passwordHashService = passwordHashService;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        User user = await _userRepository.GetByEmailAsync(request.EmailAddress, cancellationToken);
        if (user == null)
        {
            return Result.Failure<LoginResponse>(InvalidEmail);
        }

        bool isValidPassword = _passwordHashService.VerifyHashedPassword(
            user.Password.Value,
            request.Password);

        if (!isValidPassword)
        {
            return Result.Failure<LoginResponse>(InvalidPassword);
        }

        var token = _jwtService.TokenGenerator(
            user.Id,
            user.UserName.Value,
            user.EmailAddress.Value,
            user.Password.Value);

        string refreshToken = await _refreshTokenService.RefreshTokenGeneratorAsync(user.Id, cancellationToken);

        var response = new LoginResponse(token.Value, refreshToken);

        return Result.Success(response);

    }
}
