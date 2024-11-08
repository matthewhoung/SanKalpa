using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SanKalpa.Application.Users.Information;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SanKalpa.Api.Controller;

[ApiController]
[Route("api/user")]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(
        ISender sender)
    {
        _sender = sender;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetUserInfoAsync(CancellationToken cancellationToken)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var parsedUserId))
        {
            return Unauthorized();
        }

        var query = new UserInfoQuery(parsedUserId);
        var result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }
}
