using System.Collections.Concurrent;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Core services (don't modify)
builder.Services.AddSingleton<GatePassStore>();
builder.Services.AddSingleton<RfidEventBus>();
builder.Services.AddHostedService<RfidReadSimulator>();

// Your consumer — implementation lives in RfidConsumer.cs
builder.Services.AddHostedService<RfidConsumer>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// ============================================================
// EXISTING ENDPOINTS (don't modify)
// ============================================================

// GET /api/gate-pass/{id}
app.MapGet("/api/gate-pass/{id:int}", (int id, GatePassStore store) =>
{
    var pass = store.GetById(id);
    return pass is null
        ? Results.NotFound(new { error = $"Gate pass {id} not found." })
        : Results.Ok(pass);
});

// GET /api/gate-pass?status=Active
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
app.MapPost("/api/gate-pass", (CreateGatePassRequest req, GatePassStore store) =>
{
    if (string.IsNullOrWhiteSpace(req.VehicleNumber) || string.IsNullOrWhiteSpace(req.DriverName))
    {
        return Results.BadRequest(new { error = "VehicleNumber and DriverName are required." });
    }

    var pass = store.Create(req.VehicleNumber, req.DriverName, req.RfidTag ?? "");
    return Results.Created($"/api/gate-pass/{pass.Id}", pass);
});

// ============================================================
// TASK 1 (warm-up, ~5 min):
//
// Add a `POST /api/gate-pass/{id}/exit` endpoint that marks a
// gate pass as exited.
//
//   - A gate pass can only be exited if it is currently "Active".
//   - Set ExitTime to the current UTC time.
//   - Return the updated gate pass.
//
// See README.md for context.
// ============================================================



// ============================================================
// TASK 2 (main, ~20-25 min): See RfidConsumer.cs
// ============================================================

app.Run();

// ============================================================
// MODELS
// ============================================================

public record CreateGatePassRequest(string VehicleNumber, string DriverName, string? RfidTag);

public record RfidRead(string RfidTag, string ReaderId, DateTime Timestamp);

public class GatePass
{
    public int Id { get; set; }
    public string VehicleNumber { get; set; } = "";
    public string DriverName { get; set; } = "";
    public string RfidTag { get; set; } = "";
    public DateTime EntryTime { get; set; }
    public DateTime? ExitTime { get; set; }
    public string Status { get; set; } = "Active";   // "Active", "Exited", "Cancelled"
}

// ============================================================
// GATE PASS STORE (don't modify)
// ============================================================
public class GatePassStore
{
    private readonly ConcurrentDictionary<int, GatePass> _passes = new();
    private int _nextId = 0;

    public GatePassStore()
    {
        // Seed gate passes — these are what the RFID simulator will reference.
        Create("HR55-AB-1234", "Ramesh Kumar", "TAG-A001");
        Create("DL01-CD-5678", "Suresh Singh", "TAG-A002");

        var cancelled = Create("UP16-EF-9012", "Vikram Sharma", "TAG-A003");
        cancelled.Status = "Cancelled";

        var alreadyExited = Create("RJ14-GH-3456", "Anil Yadav", "TAG-A004");
        alreadyExited.Status = "Exited";
        alreadyExited.ExitTime = DateTime.UtcNow.AddHours(-1);
    }

    public GatePass? GetById(int id) => _passes.TryGetValue(id, out var p) ? p : null;

    public GatePass? FindByRfidTag(string rfidTag) =>
        _passes.Values.FirstOrDefault(p =>
            p.RfidTag.Equals(rfidTag, StringComparison.OrdinalIgnoreCase));

    public List<GatePass> GetAll() => _passes.Values.ToList();

    public GatePass Create(string vehicleNumber, string driverName, string rfidTag = "")
    {
        var id = Interlocked.Increment(ref _nextId);
        var pass = new GatePass
        {
            Id = id,
            VehicleNumber = vehicleNumber,
            DriverName = driverName,
            RfidTag = rfidTag,
            EntryTime = DateTime.UtcNow,
            Status = "Active"
        };
        _passes[id] = pass;
        return pass;
    }
}

// ============================================================
// RFID EVENT BUS (don't modify)
//
// Thin wrapper around a Channel<RfidRead>. The simulator writes,
// your consumer reads.
// ============================================================
public class RfidEventBus
{
    private readonly Channel<RfidRead> _channel =
        Channel.CreateUnbounded<RfidRead>();

    public ChannelWriter<RfidRead> Writer => _channel.Writer;
    public ChannelReader<RfidRead> Reader => _channel.Reader;
}

// ============================================================
// RFID READ SIMULATOR (don't modify)
//
// Emits a scripted sequence of RFID reads every ~3 seconds.
// Loops indefinitely. Watch the console to see what's emitted.
// ============================================================
public class RfidReadSimulator : BackgroundService
{
    private readonly RfidEventBus _bus;
    private readonly ILogger<RfidReadSimulator> _logger;

    // Scripted sequence emitted on a 3-second cadence, loops indefinitely.
    private static readonly (string Tag, string Reader)[] Script = new[]
    {
        ("TAG-A001", "EXIT-GATE"),
        ("TAG-A001", "EXIT-GATE"),
        ("TAG-X999", "EXIT-GATE"),
        ("TAG-A003", "EXIT-GATE"),
        ("TAG-A002", "EXIT-GATE"),
    };

    public RfidReadSimulator(RfidEventBus bus, ILogger<RfidReadSimulator> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Brief warm-up so the candidate can see startup logs before events flow.
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        var index = 0;
        while (!stoppingToken.IsCancellationRequested)
        {
            var (tag, reader) = Script[index % Script.Length];
            var read = new RfidRead(tag, reader, DateTime.UtcNow);

            await _bus.Writer.WriteAsync(read, stoppingToken);
            _logger.LogInformation(
                "[Simulator] Emitted RFID read: {Tag} @ {Reader}",
                read.RfidTag, read.ReaderId);

            index++;
            await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
        }
    }
}
