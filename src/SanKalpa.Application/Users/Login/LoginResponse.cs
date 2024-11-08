namespace SanKalpa.Application.Users.Login;

public sealed record class LoginResponse(string AccessToken, string RefreshToken);
