using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<GatePassStore>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// ============================================================
// Endpoints
// ============================================================

// GET /api/gate-pass/{id}
//   Returns a single gate pass by id.
app.MapGet("/api/gate-pass/{id:int}", (int id, GatePassStore store) =>
{
    var pass = store.GetById(id);
    return pass is null
        ? Results.NotFound(new { error = $"Gate pass {id} not found." })
        : Results.Ok(pass);
});

// GET /api/gate-pass
//   Returns all gate passes, optionally filtered by status.
app.MapGet("/api/gate-pass", (string? status, GatePassStore store) =>
{
    var all = store.GetAll();
    if (!string.IsNullOrWhiteSpace(status))
    {
        all = all.Where(p => p.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
    }
    return Results.Ok(all);
});

// POST /api/gate-pass
//   Creates a new gate pass. EntryTime is set server-side. Status starts as "Active".
app.MapPost("/api/gate-pass", (CreateGatePassRequest req, GatePassStore store) =>
{
    if (string.IsNullOrWhiteSpace(req.VehicleNumber) || string.IsNullOrWhiteSpace(req.DriverName))
    {
        return Results.BadRequest(new { error = "VehicleNumber and DriverName are required." });
    }

    var pass = store.Create(req.VehicleNumber, req.DriverName);
    return Results.Created($"/api/gate-pass/{pass.Id}", pass);
});

// ============================================================
// TODO (candidate task):
//
// Add a `POST /api/gate-pass/{id}/exit` endpoint that marks a
// gate pass as exited.
//
// A gate pass can only be exited if it is currently active.
// Set the exit time to the current time.
// Return the updated gate pass.
//
// See README.md for the full task statement.
// ============================================================

app.Run();

// ============================================================
// Models
// ============================================================

public record CreateGatePassRequest(string VehicleNumber, string DriverName);

public class GatePass
{
    public int Id { get; set; }
    public string VehicleNumber { get; set; } = "";
    public string DriverName { get; set; } = "";
    public DateTime EntryTime { get; set; }
    public DateTime? ExitTime { get; set; }
    public string Status { get; set; } = "Active";   // "Active", "Exited", "Cancelled"
}

// ============================================================
// In-memory store (no database setup needed)
// ============================================================
public class GatePassStore
{
    private readonly ConcurrentDictionary<int, GatePass> _passes = new();
    private int _nextId = 0;

    public GatePassStore()
    {
        // Seed a few sample gate passes for testing
        Create("HR55-AB-1234", "Ramesh Kumar");
        Create("DL01-CD-5678", "Suresh Singh");

        var cancelled = Create("UP16-EF-9012", "Vikram Sharma");
        cancelled.Status = "Cancelled";

        var exited = Create("RJ14-GH-3456", "Anil Yadav");
        exited.Status = "Exited";
        exited.ExitTime = DateTime.UtcNow.AddHours(-1);
    }

    public GatePass? GetById(int id) => _passes.TryGetValue(id, out var p) ? p : null;

    public List<GatePass> GetAll() => _passes.Values.ToList();

    public GatePass Create(string vehicleNumber, string driverName)
    {
        var id = Interlocked.Increment(ref _nextId);
        var pass = new GatePass
        {
            Id = id,
            VehicleNumber = vehicleNumber,
            DriverName = driverName,
            EntryTime = DateTime.UtcNow,
            Status = "Active"
        };
        _passes[id] = pass;
        return pass;
    }
}
