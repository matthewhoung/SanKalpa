using SanKalpa.Application.Abstrations.Messaging;

namespace SanKalpa.Application.Users.Information;

public sealed record class UserInfoQuery(Guid UserId) : IQuery<UserInfoQueryResponse>;
