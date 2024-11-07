using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SanKalpa.Application.Users.Information;

namespace SanKalpa.Api.Controller;

[ApiController]
[Route("api/user")]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserInfoAsync(
        [FromQuery] UserInfoQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(query);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }
}
