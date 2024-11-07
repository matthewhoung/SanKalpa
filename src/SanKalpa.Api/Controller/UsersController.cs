using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SanKalpa.Api.Controller
{
    [ApiController]
    [Route("api/user")]
    public class UsersController : ControllerBase
    {
        private readonly ISender _sender;

        public UsersController(ISender sender)
        {
            _sender = sender;
        }

    }
}
