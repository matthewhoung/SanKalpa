using SanKalpa.Application.Abstrations.Authentication;
using SanKalpa.Domain.Abstrations;
using SanKalpa.Domain.Services;
using SanKalpa.Domain.Users;
using SanKalpa.Domain.Users.ValueObjects;

namespace SanKalpa.Infrastructure.Authentication;

internal sealed class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IJwtService _jwtService;
    private readonly IUnitOfWork _unitOfWork;

    public AuthenticationService(
        IUserRepository userRepository,
        IPasswordHashService passwordHashService,
        IJwtService jwtService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHashService = passwordHashService;
        _jwtService = jwtService;
        _unitOfWork = unitOfWork;
    }

    public async Task<string> RegisterAsync(User user, CancellationToken cancellationToken = default)
    {
        string passwordHash = _passwordHashService.HashPassword(user.Password.Value);

        User register = User.Create(
            user.UserName,
            user.EmailAddress,
            new Password(passwordHash));

        _userRepository.Add(register);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = _jwtService.TokenGenerator(
            register.Id,
            register.UserName.Value,
            register.EmailAddress.Value,
            register.Password.Value);

        return token.Value;
    }
}
