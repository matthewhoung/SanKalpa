using SanKalpa.Application.Abstrations.Messaging;

namespace SanKalpa.Application.Users.Register;

public sealed record class RegistrationCommand(
    string UserName,
    string EmailAddress,
    string Password) : ICommand<string>;
