// ============================================================
// TASK 2 — RFID event consumer
// ============================================================
//
// The interviewer has explained the domain. This file is the
// technical surface you need to work against.
//
//
// WHAT'S WIRED UP FOR YOU
// -----------------------
//   RfidReadSimulator  (in Program.cs)
//       Emits RfidRead events into the bus every ~3 sec.
//       You don't need to modify it. Watch the console to see
//       what's being emitted.
//
//   RfidEventBus       (in Program.cs)
//       Thin wrapper around Channel<RfidRead>.
//       Read events via:  _bus.Reader
//
//   GatePassStore      (in Program.cs)
//       In-memory store of gate passes. Useful methods:
//         _store.FindByRfidTag(tag)   -> GatePass? (case-insensitive)
//         _store.GetById(id)          -> GatePass?
//         _store.GetAll()             -> List<GatePass>
//       Gate passes have a Status field: "Active", "Exited", "Cancelled".
//
//   RfidRead
//       record RfidRead(string RfidTag, string ReaderId, DateTime Timestamp)
//       For this exercise, ReaderId is always "EXIT-GATE".
//
//
// WHAT TO BUILD
// -------------
// Implement ExecuteAsync below.
//
// Minimum spec:
//   For each RfidRead consumed from the bus:
//     - Find the matching gate pass by RfidTag.
//     - If found and currently "Active":
//         set Status = "Exited", ExitTime = read.Timestamp.
//     - Otherwise: log what happened and continue.
//
// Edge cases worth thinking about (handle the ones you think
// matter, be ready to talk about the rest):
//   - The tag matches no gate pass.
//   - The matched pass is not Active (already Exited, Cancelled).
//   - The same tag is read multiple times in quick succession.
//   - An exception in one read shouldn't kill the consumer.
//   - An HTTP request and your consumer could try to update the
//     same pass at the same time.
//
//
// SCOPE
// -----
// Don't modify Program.cs (other than Task 1) or any of the
// supporting services. Just implement ExecuteAsync below; add
// private methods/fields here as needed.
// ============================================================

public class RfidConsumer : BackgroundService
{
    private readonly RfidEventBus _bus;
    private readonly GatePassStore _store;
    private readonly ILogger<RfidConsumer> _logger;

    public RfidConsumer(
        RfidEventBus bus,
        GatePassStore store,
        ILogger<RfidConsumer> logger)
    {
        _bus = bus;
        _store = store;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // TODO: implement the consumer here.
        await Task.CompletedTask;
    }
}
