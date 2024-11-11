using SanKalpa.Application.Abstrations.Authentication;
using SanKalpa.Application.Abstrations.Messaging;
using SanKalpa.Domain.Abstrations;
using SanKalpa.Domain.Users;
using SanKalpa.Domain.Users.ValueObjects;

namespace SanKalpa.Application.Users.Register;

internal sealed class RegistrationCommandHandler : ICommandHandler<RegistrationCommand, RegisterResponse>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IRefreshTokenService _refreshTokenService;

    public RegistrationCommandHandler(
        IAuthenticationService authenticationService,
        IRefreshTokenService refreshTokenService)
    {
        _authenticationService = authenticationService;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<Result<RegisterResponse>> Handle(RegistrationCommand request, CancellationToken cancellationToken)
    {
        User userInfo = User.Create(
            new UserName(request.UserName),
            new EmailAddress(request.EmailAddress),
            new Password(request.Password));

        string token = await _authenticationService.RegisterAsync(userInfo, cancellationToken);
        string refreshToken = await _refreshTokenService.RefreshTokenGeneratorAsync(userInfo.Id, cancellationToken);

        var response = new RegisterResponse(token, refreshToken);

        return Result.Success(response);
    }
}
