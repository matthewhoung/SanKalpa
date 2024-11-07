using MediatR;
using Microsoft.AspNetCore.Mvc;
using SanKalpa.Application.Users.Register;

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

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(
        [FromBody] RegistrationCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command);

        return result.IsSuccess
            ? Ok(result.Value)
            : BadRequest(result.Error);
    }

}
