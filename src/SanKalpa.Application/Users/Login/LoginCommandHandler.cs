using Microsoft.AspNetCore.Http;
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
    private readonly ILoginAttemptService _loginAttemptService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private static readonly Error InvalidCredentials = new(
        "AUTH001",
        "Invalid email or password");

    private static readonly Error TooManyAttempts = new(
        "AUTH002",
        "Too many failed login attempts. Please try again later.");

    public LoginCommandHandler(
        IJwtService jwtService,
        IUserRepository userRepository,
        IPasswordHashService passwordHashService,
        IRefreshTokenService refreshTokenService,
        ILoginAttemptService loginAttemptService,
        IHttpContextAccessor httpContextAccessor)
    {
        _jwtService = jwtService;
        _userRepository = userRepository;
        _passwordHashService = passwordHashService;
        _refreshTokenService = refreshTokenService;
        _loginAttemptService = loginAttemptService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // getting IP for tracking
            string? ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
            if (!await _loginAttemptService.AttemptLoginAsync(request.EmailAddress, cancellationToken))
            {
                return Result.Failure<LoginResponse>(TooManyAttempts);
            }

            if (string.IsNullOrWhiteSpace(request.EmailAddress) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return Result.Failure<LoginResponse>(InvalidCredentials);
            }

            // Use timing-safe comparison to prevent timing attacks
            User? user = await _userRepository.GetByEmailAsync(request.EmailAddress, cancellationToken);
            if (user == null)
            {
                return Result.Failure<LoginResponse>(InvalidCredentials);
            }

            bool isValidPassword = _passwordHashService.VerifyHashedPassword(
                user.Password.Value,
                request.Password);

            if (!isValidPassword)
            {
                return Result.Failure<LoginResponse>(InvalidCredentials);
            }

            await _loginAttemptService.RecordLoginAttemptAsync(
                request.EmailAddress,
                true,
                ipAddress,
                cancellationToken);

            // Generate tokens
            var token = _jwtService.TokenGenerator(
                user.Id,
                user.UserName.Value,
                user.EmailAddress.Value,
                user.Password.Value);

            string refreshToken = await _refreshTokenService.RefreshTokenGeneratorAsync(
                user.Id,
                cancellationToken);

            var response = new LoginResponse(token.Value, refreshToken);
            return Result.Success(response);
        }
        catch
        {
            return Result.Failure<LoginResponse>(new Error(
                "AUTH500",
                "An unexpected error occurred during login"));
        }
    }
}
