namespace GateAutomationLive;

public record CreateGatePassRequest(string VehicleNumber, string DriverName, string? RfidTag);

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
