namespace consolezebra.Models;

public class ScannerResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? BarcodeData { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

public class ScaleResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? WeightStatus { get; set; }
    public double Weight { get; set; }
    public string? Unit { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

public class DeviceStatusResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public bool ScannerConnected { get; set; }
    public bool ScaleConnected { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

public class InitializeResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
