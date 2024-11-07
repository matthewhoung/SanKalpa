using Dapper;
using SanKalpa.Application.Abstrations.Data;
using SanKalpa.Application.Abstrations.Messaging;
using SanKalpa.Domain.Abstrations;

namespace SanKalpa.Application.Users.Information;

internal sealed class UserInfoQueryHandler : IQueryHandler<UserInfoQuery, UserInfoQueryResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public UserInfoQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<UserInfoQueryResponse>> Handle(
        UserInfoQuery request, 
        CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        var parameters = new
        {
            UserId = request.UserId
        };

        const string sql = """
            SELECT
                Id AS UserId,
                user_name AS UserName,
                email_address AS EmailAddress,
                created_at AS CreatedAt
            FROM
                users
            WHERE
                id = @UserId
            """;

        var result = await connection.QuerySingleOrDefaultAsync<UserInfoQueryResponse>(
            sql,
            parameters);

        var response = new UserInfoQueryResponse
        {
            UserId = result.UserId,
            UserName = result.UserName,
            EmailAddress = result.EmailAddress,
            CreatedAt = result.CreatedAt
        };

        return Result<UserInfoQueryResponse>.Success(response);
    }
}
