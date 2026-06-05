using System.Collections.Concurrent;

namespace GateAutomationLive.Infrastructure;

public class GatePassStore
{
    private readonly ConcurrentDictionary<int, GatePass> _passes = new();
    private int _nextId = 0;

    public GatePassStore()
    {
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
