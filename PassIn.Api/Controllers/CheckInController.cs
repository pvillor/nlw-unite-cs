using Microsoft.AspNetCore.Mvc;
using PassIn.Application.UseCases.CheckIns;
using PassIn.Communication.Responses;

namespace PassIn.Api.Controllers;

[Route("api/attendees/{attendeeId}/[controller]")]
[ApiController]
public class CheckInController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ResponseAllAttendeesJson), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status409Conflict)]
    public IActionResult CheckIn([FromRoute] Guid attendeeId)
    {
        var useCase = new CheckInUseCase();

        var response = useCase.Execute(attendeeId);

        return Created(string.Empty, response);
    }
}
