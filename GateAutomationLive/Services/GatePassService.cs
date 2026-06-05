using GateAutomationLive.Infrastructure;

namespace GateAutomationLive.Services;

public class GatePassService : IGatePassService
{
    private readonly GatePassStore _store;
    private readonly ILogger<GatePassService> _logger;

    public GatePassService(GatePassStore store, ILogger<GatePassService> logger)
    {
        _store = store;
        _logger = logger;
    }

    public GatePass? GetById(int id) => _store.GetById(id);

    public GatePass? FindByRfidTag(string rfidTag) => _store.FindByRfidTag(rfidTag);

    public List<GatePass> GetAll(string? statusFilter = null)
    {
        var all = _store.GetAll();
        if (!string.IsNullOrWhiteSpace(statusFilter))
        {
            all = all.Where(p =>
                p.Status.Equals(statusFilter, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        return all;
    }

    public GatePass Create(CreateGatePassRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.VehicleNumber))
            throw new ArgumentException("VehicleNumber is required.", nameof(request));
        if (string.IsNullOrWhiteSpace(request.DriverName))
            throw new ArgumentException("DriverName is required.", nameof(request));

        var pass = _store.Create(request.VehicleNumber, request.DriverName, request.RfidTag ?? "");
        _logger.LogInformation("Created gate pass {Id} for {Vehicle}", pass.Id, pass.VehicleNumber);
        return pass;
    }

    // ============================================================
    // TASK 1 — Add your implementation here.
    // ============================================================
}
