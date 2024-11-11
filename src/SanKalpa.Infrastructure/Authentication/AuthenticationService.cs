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
        var existingUser = await _userRepository.GetByEmailAsync(user.EmailAddress.Value, cancellationToken);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Email address is already registered");
        }

        string passwordHash = _passwordHashService.HashPassword(user.Password.Value);
        user.SetPassword(new Password(passwordHash));
        _userRepository.Add(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = _jwtService.TokenGenerator(
            user.Id,
            user.UserName.Value,
            user.EmailAddress.Value,
            user.Password.Value);

        return token.Value;
    }
}
