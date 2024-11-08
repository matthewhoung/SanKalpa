using SanKalpa.Application.Abstrations.Messaging;

namespace SanKalpa.Application.Users.Login;

public sealed record class LoginCommand(string EmailAddress, string Password) : ICommand<LoginResponse>;
