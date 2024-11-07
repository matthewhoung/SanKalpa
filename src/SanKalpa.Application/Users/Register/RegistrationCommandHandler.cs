using SanKalpa.Application.Abstrations.Authentication;
using SanKalpa.Application.Abstrations.Messaging;
using SanKalpa.Domain.Abstrations;
using SanKalpa.Domain.Users;
using SanKalpa.Domain.Users.ValueObjects;

namespace SanKalpa.Application.Users.Register;

internal sealed class RegistrationCommandHandler : ICommandHandler<RegistrationCommand, string>
{
    private readonly IAuthenticationService _authenticationService;

    public RegistrationCommandHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    public async Task<Result<string>> Handle(RegistrationCommand request, CancellationToken cancellationToken)
    {
        User userInfo = User.Create(
            new UserName(request.UserName),
            new EmailAddress(request.EmailAddress),
            new Password(request.Password));

        string token = await _authenticationService.RegisterAsync(userInfo, cancellationToken);

        return Result<string>.Success(token);
    }
}
