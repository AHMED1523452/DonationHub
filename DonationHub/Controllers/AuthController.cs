
using DonationHub.Application.Features.Auth.Commands;
using DonationHub.Application.Features.Auth.DTOs;
using DonationHub.Application.Features.Auth.Queries;
using DonationHub.Application.Patterns;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator mediator;

        public AuthController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(GenericResult<RegisterResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenericResult<RegisterResponseDTO>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GenericResult<RegisterResponseDTO>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request,
                                                  CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new RegisterCommand(request), cancellationToken);

            if (result.IsSuccess is not true)
                return StatusCode(result.StatusCode ?? StatusCodes.Status400BadRequest, result);

            return Ok(result);
        }

        [HttpPost("create-admin-or-staff")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(GenericResult<CreateAdminOrStaffResponseDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(GenericResult<CreateAdminOrStaffResponseDTO>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(GenericResult<CreateAdminOrStaffResponseDTO>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAdminOrStaff([FromBody] CreateAdminOrStaffCommand command,
                                                            CancellationToken cancellationToken)
        {
            var result = await mediator.Send(command, cancellationToken);

            if (result.IsSuccess is not true)
                return StatusCode(result.StatusCode ?? StatusCodes.Status400BadRequest, result);

            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(GenericResult<LoginResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenericResult<LoginResponseDTO>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GenericResult<LoginResponseDTO>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GenericResult<LoginResponseDTO>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request,
                                               CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new LoginCommand(request), cancellationToken);

            if (result.IsSuccess is not true)
                return StatusCode(result.StatusCode ?? StatusCodes.Status400BadRequest, result);

            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(typeof(GenericResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new LogoutCommand(), cancellationToken);

            if (result.IsSuccess is not true)
                return StatusCode(result.StatusCode ?? StatusCodes.Status400BadRequest, result);

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(GenericResult<RefreshTokenRequestDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GenericResult<RefreshTokenRequestDTO>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDTO request,
                                                     CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new RefreshTokenCommand(request), cancellationToken);

            if (result.IsSuccess is not true)
                return StatusCode(result.StatusCode ?? StatusCodes.Status400BadRequest, result);

            return Ok(result);
        }

        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(GenericResult<UserProfileResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Me(CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new AuthMeQuery(), cancellationToken);

            if (result.IsSuccess is not true)
                return StatusCode(result.StatusCode ?? StatusCodes.Status400BadRequest, result);

            return Ok(result);
        }
    }
}
