using GateAutomationLive.Services;
using Microsoft.AspNetCore.Mvc;

namespace GateAutomationLive.Controllers;

[ApiController]
[Route("api/gate-pass")]
public class GatePassController : ControllerBase
{
    private readonly IGatePassService _service;
    private readonly ILogger<GatePassController> _logger;

    public GatePassController(IGatePassService service, ILogger<GatePassController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // GET /api/gate-pass/{id}
    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var pass = _service.GetById(id);
        return pass is null
            ? NotFound(new { error = $"Gate pass {id} not found." })
            : Ok(pass);
    }

    // GET /api/gate-pass?status=Active
    [HttpGet]
    public IActionResult GetAll([FromQuery] string? status)
    {
        var passes = _service.GetAll(status);
        return Ok(passes);
    }

    // POST /api/gate-pass
    [HttpPost]
    public IActionResult Create([FromBody] CreateGatePassRequest request)
    {
        try
        {
            var pass = _service.Create(request);
            return CreatedAtAction(nameof(GetById), new { id = pass.Id }, pass);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // ============================================================
    // TASK 1 (~5 min) — Add the exit endpoint here.
    //
    // POST /api/gate-pass/{id}/exit
    //
    // See README.md for the full spec. You may need to extend
    // IGatePassService and GatePassService as well.
    // ============================================================
}
