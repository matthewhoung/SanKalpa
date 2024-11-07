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

    [HttpGet("debug-auth")]
    public IActionResult DebugAuth()
    {
        var headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());

        var authHeader = Request.Headers["Authorization"].ToString();

        var claims = User.Claims.Select(c => new
        {
            Type = c.Type,
            Value = c.Value
        }).ToList();

        // Get specific important claims
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        var name = User.FindFirst(ClaimTypes.Name)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value
            ?? User.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

        var response = new
        {
            IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
            AuthenticationType = User.Identity?.AuthenticationType,
            Headers = headers,
            AuthorizationHeader = authHeader,
            Claims = claims,
            ImportantClaims = new
            {
                UserId = userId,
                Name = name,
                Email = email
            }
        };

        return Ok(response);
    }
}
