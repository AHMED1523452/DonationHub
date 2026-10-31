using DonationHub.Application.Features.Category.Commands;
using DonationHub.Application.Features.Category.DTOs;
using DonationHub.Application.Features.Category.Queries;
using DonationHub.Application.Patterns;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Controllers
{
    [ApiController]
    [Route("api/v1/categories")]
    [Produces("application/json")]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator mediator;

        public CategoryController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(GenericResult<CategoryResponseDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(GenericResult<CategoryResponseDTO>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(GenericResult<CategoryResponseDTO>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequestDTO request,
                                               CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new CreateCategoryCommand(request), cancellationToken);

            if (result.IsSuccess is not true)
                return StatusCode(result.StatusCode ?? StatusCodes.Status400BadRequest, result);

            return StatusCode(StatusCodes.Status201Created, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(GenericResult<CategoryResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenericResult<CategoryResponseDTO>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(GenericResult<CategoryResponseDTO>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GenericResult<CategoryResponseDTO>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromRoute] Guid id,
                                                [FromBody] UpdateCategoryRequestDTO request,
                                                CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new UpdateCategoryCommand(id, request), cancellationToken);

            if (result.IsSuccess is not true)
                return StatusCode(result.StatusCode ?? StatusCodes.Status400BadRequest, result);

            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(GenericResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(GenericResult<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GenericResult<string>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete([FromRoute] Guid id,
                                                CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new DeleteCategoryCommand(id), cancellationToken);

            if (result.IsSuccess is not true)
                return StatusCode(result.StatusCode ?? StatusCodes.Status400BadRequest, result);

            return Ok(result);
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PaginatedResult<CategoryResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1,
                                                [FromQuery] int pageSize = 10,
                                                [FromQuery] string? searchTerm = null,
                                                CancellationToken cancellationToken = default)
        {
            var result = await mediator.Send(new GetCategoriesQuery(page, pageSize, searchTerm), cancellationToken);

            if (result.IsSuccess is not true)
                return StatusCode(StatusCodes.Status400BadRequest, result);

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(GenericResult<CategoryResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(GenericResult<CategoryResponseDTO>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById([FromRoute] Guid id,
                                                 CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetCategoryByIdQuery(id), cancellationToken);

            if (result.IsSuccess is not true)
                return StatusCode(result.StatusCode ?? StatusCodes.Status400BadRequest, result);

            return Ok(result);
        }
    }
}
