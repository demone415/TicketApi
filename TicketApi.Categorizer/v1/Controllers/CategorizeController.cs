using Microsoft.AspNetCore.Mvc;
using TicketApi.Entities;
using TicketApi.Interfaces.Services;

namespace TicketApi.Categorizer.v1.Controllers;

[ApiController]
[Produces("application/json")]
[Route("v1/categorize")]
public class CategorizeController : ControllerBase
{
    private readonly ICategorizationService _categorizationService;

    public CategorizeController(ICategorizationService classificationService)
    {
        _categorizationService = classificationService;
    }

    [HttpPost("")]
    public async Task<IActionResult> Categorize([FromBody] TicketHeader header, CancellationToken ct)
    {
        var classified = await _categorizationService.CategorizeTicketAsync(header, ct);
        return Ok(classified);
    }
}