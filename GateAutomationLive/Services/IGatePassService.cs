namespace GateAutomationLive.Services;

public interface IGatePassService
{
    GatePass? GetById(int id);
    List<GatePass> GetAll(string? statusFilter = null);
    GatePass? FindByRfidTag(string rfidTag);
    GatePass Create(CreateGatePassRequest request);

    // ============================================================
    // TASK 1 — Extend this interface (and GatePassService) with
    // whatever operation you need to support the new endpoint.
    // The controller should delegate to the service rather than
    // talking to the store directly.
    // ============================================================
}
