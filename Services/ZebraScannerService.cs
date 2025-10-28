using consolezebra.Models;

namespace consolezebra.Services;

public class ZebraScannerService
{
    private OPOSScannerClass? scanner;
    private OPOSScaleClass? scale;
    private bool _isInitialized = false;

    public event Action<string>? OnBarcodeDataReceived;
    public event Action<string, double>? OnWeightUpdated;

    public ZebraScannerService()
    {
        scanner = new OPOSScannerClass();
        scale = new OPOSScaleClass();
    }

    public InitializeResponse Initialize()
    {
        var response = new InitializeResponse();
        var sResult = "";

        try
        {
            scanner?.Open("ZEBRA_SCANNER");
            scanner?.ClaimDevice(1000);

            scale?.Open("ZEBRA_SCALE");
            scale?.ClaimDevice(1000);

            if (scanner?.Claimed == true)
            {
                scanner.DataEvent += Scanner_DataEvent;
                scanner.DeviceEnabled = true;
                scanner.DataEventEnabled = true;
                scanner.DecodeData = true;
            }
            else
            {
                sResult = "Failed to connect to any scanner.";
            }

            if (scale?.CapStatusUpdate == true)
            {
                scale.StatusNotify = (int)OPOSScaleConstants.SCAL_SN_ENABLED;
                if (scale.ResultCode == (int)OPOSConstants.OPOS_SUCCESS)
                {
                    scale.StatusUpdateEvent += StatusUpdateEvent;
                    scale.DeviceEnabled = true;
                    if (scale.DeviceEnabled)
                    {
                        scale.DataEventEnabled = true;
                        scale.DataEventEnabled = false;
                    }
                    else
                    {
                        sResult = "Failed to enable the scale. Error code: " + scale.ResultCode;
                    }
                }
            }
            else
            {
                sResult = "Failed to connect to any scale.";
            }

            if (string.IsNullOrEmpty(sResult))
            {
                _isInitialized = true;
                response.Success = true;
                response.Message = "Scanner and scale initialized successfully";
            }
            else
            {
                response.Success = false;
                response.Message = sResult;
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error initializing devices: {ex.Message}";
        }

        return response;
    }

    public DeviceStatusResponse GetDeviceStatus()
    {
        var response = new DeviceStatusResponse();
        
        try
        {
            response.ScannerConnected = scanner?.Claimed == true;
            response.ScaleConnected = scale?.Claimed == true;
            response.Success = true;
            response.Message = "Device status retrieved successfully";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error getting device status: {ex.Message}";
        }

        return response;
    }

    public ScannerResponse GetLastBarcode()
    {
        var response = new ScannerResponse();
        
        try
        {
            if (!_isInitialized)
            {
                response.Success = false;
                response.Message = "Scanner not initialized";
                return response;
            }

            if (scanner?.Claimed == true)
            {
                response.Success = true;
                response.BarcodeData = scanner.ScanDataLabel;
                response.Message = "Barcode data retrieved successfully";
            }
            else
            {
                response.Success = false;
                response.Message = "Scanner not connected";
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error getting barcode data: {ex.Message}";
        }

        return response;
    }

    public ScaleResponse GetCurrentWeight()
    {
        var response = new ScaleResponse();
        
        try
        {
            if (!_isInitialized)
            {
                response.Success = false;
                response.Message = "Scale not initialized";
                return response;
            }

            if (scale?.Claimed == true)
            {
                response.Success = true;
                response.Weight = scale.ScaleLiveWeight;
                response.WeightStatus = WeightFormat(scale.ScaleLiveWeight);
                response.Unit = UnitAbbreviation(scale.WeightUnits);
                response.Message = "Weight data retrieved successfully";
            }
            else
            {
                response.Success = false;
                response.Message = "Scale not connected";
            }
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = $"Error getting weight data: {ex.Message}";
        }

        return response;
    }

    private void Scanner_DataEvent(int Status)
    {
        scanner.DataEventEnabled = true;
        OnBarcodeDataReceived?.Invoke(scanner.ScanDataLabel);
    }

    private void StatusUpdateEvent(int value)
    {
        string weightStatus = string.Empty;

        switch (value)
        {
            case (int)OPOSScaleConstants.SCAL_SUE_STABLE_WEIGHT:
                weightStatus = WeightFormat(scale.ScaleLiveWeight);
                break;

            case (int)OPOSScaleConstants.SCAL_SUE_WEIGHT_UNSTABLE:
                weightStatus = ".......................";
                break;

            case (int)OPOSScaleConstants.SCAL_SUE_WEIGHT_ZERO:
                weightStatus = WeightFormat(scale.ScaleLiveWeight);
                break;

            case (int)OPOSScaleConstants.SCAL_SUE_WEIGHT_OVERWEIGHT:
                weightStatus = "Weight limit exceeded.";
                break;

            case (int)OPOSScaleConstants.SCAL_SUE_NOT_READY:
                weightStatus = "Scale not ready.";
                break;

            case (int)OPOSScaleConstants.SCAL_SUE_WEIGHT_UNDER_ZERO:
                weightStatus = "Scale under zero weight.";
                break;

            default:
                weightStatus = $"Unknown status [{value}]";
                break;
        }

        OnWeightUpdated?.Invoke(weightStatus, scale.ScaleLiveWeight);
    }

    private string WeightFormat(int weight)
    {
        string units = UnitAbbreviation(scale.WeightUnits);
        return string.IsNullOrEmpty(units) ? "Unknown weight unit" : $"{0.001 * weight:0.000} {units}";
    }

    private string UnitAbbreviation(int units)
    {
        switch(units)
        {
            case (int)OPOSScaleConstants.SCAL_WU_GRAM:
                return "gr.";
            case (int)OPOSScaleConstants.SCAL_WU_KILOGRAM:
                return "Kg.";
            case (int)OPOSScaleConstants.SCAL_WU_OUNCE:
                return "oz.";
            case (int)OPOSScaleConstants.SCAL_WU_POUND:
                return "lb.";
            default:
                return string.Empty;
        }
    }

    public void Close()
    {
        try
        {
            scanner.DataEvent -= Scanner_DataEvent;
            scanner.DataEventEnabled = false;
            scanner.ReleaseDevice();
            scanner.Close();

            scale.StatusUpdateEvent -= StatusUpdateEvent;
            scale.DataEventEnabled = false;
            scale.ReleaseDevice();
            scale.Close();

            _isInitialized = false;
        }
        catch (Exception ex)
        {
            // Log error if needed
            Console.WriteLine($"Error closing devices: {ex.Message}");
        }
    }
}
