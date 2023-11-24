using Microsoft.AspNetCore.Mvc;
using TicketApi.Entities;
using TicketApi.Interfaces.Services;

namespace TicketApi.Categorizer.Controllers;

[ApiController]
[Produces("application/json")]
[Route("[controller]")]
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