using SanKalpa.Application.Abstrations.Authentication;
using SanKalpa.Application.Abstrations.Messaging;
using SanKalpa.Domain.Abstrations;

namespace SanKalpa.Application.Users.Login;

internal sealed class LoginCommandHandler : ICommandHandler<LoginCommand, string>
{
    private readonly IAuthenticationService _authenticationService;

    public LoginCommandHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<Result<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        string token = await _authenticationService.LoginAsync(
            request.EmailAddress,
            request.Password,
            cancellationToken);

        return Result<string>.Success(token);
    }
}
