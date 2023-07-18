using Microsoft.AspNetCore.Mvc;
using TicketApi.Entities;
using TicketApi.Interfaces.Services;

namespace TicketApi.Classifier.Controllers;

[ApiController]
[Produces("application/json")]
[Route("[controller]")]
public class ClassifyController : ControllerBase
{
    private readonly IClassificationService _classificationService;
    public ClassifyController(IClassificationService classificationService)
    {
        _classificationService = classificationService;
    }
    
    [HttpGet("")]
    public async Task<IActionResult> Classify(TicketHeader header)
    {
        var classified = await _classificationService.Classify(header);
        return Ok(classified);
    }
}