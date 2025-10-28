using Microsoft.AspNetCore.Mvc;
using consolezebra.Models;
using consolezebra.Services;

namespace consolezebra.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ZebraController : ControllerBase
{
    private readonly ZebraScannerService _zebraService;
    private readonly ILogger<ZebraController> _logger;

    public ZebraController(ZebraScannerService zebraService, ILogger<ZebraController> logger)
    {
        _zebraService = zebraService;
        _logger = logger;
    }

    /// <summary>
    /// Initialize the Zebra scanner and scale devices
    /// </summary>
    /// <returns>Initialization result</returns>
    [HttpPost("initialize")]
    public ActionResult<InitializeResponse> Initialize()
    {
        try
        {
            _logger.LogInformation("Initializing Zebra devices...");
            var result = _zebraService.Initialize();
            
            if (result.Success)
            {
                _logger.LogInformation("Zebra devices initialized successfully");
                return Ok(result);
            }
            else
            {
                _logger.LogWarning("Failed to initialize Zebra devices: {Message}", result.Message);
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing Zebra devices");
            return StatusCode(500, new InitializeResponse 
            { 
                Success = false, 
                Message = $"Internal server error: {ex.Message}" 
            });
        }
    }

    /// <summary>
    /// Get the current status of scanner and scale devices
    /// </summary>
    /// <returns>Device status information</returns>
    [HttpGet("status")]
    public ActionResult<DeviceStatusResponse> GetStatus()
    {
        try
        {
            _logger.LogInformation("Getting device status...");
            var result = _zebraService.GetDeviceStatus();
            
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting device status");
            return StatusCode(500, new DeviceStatusResponse 
            { 
                Success = false, 
                Message = $"Internal server error: {ex.Message}" 
            });
        }
    }

    /// <summary>
    /// Get the last scanned barcode data
    /// </summary>
    /// <returns>Barcode data</returns>
    [HttpGet("scanner/barcode")]
    public ActionResult<ScannerResponse> GetBarcode()
    {
        try
        {
            _logger.LogInformation("Getting barcode data...");
            var result = _zebraService.GetLastBarcode();
            
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting barcode data");
            return StatusCode(500, new ScannerResponse 
            { 
                Success = false, 
                Message = $"Internal server error: {ex.Message}" 
            });
        }
    }

    /// <summary>
    /// Get the current weight from the scale
    /// </summary>
    /// <returns>Current weight data</returns>
    [HttpGet("scale/weight")]
    public ActionResult<ScaleResponse> GetWeight()
    {
        try
        {
            _logger.LogInformation("Getting weight data...");
            var result = _zebraService.GetCurrentWeight();
            
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting weight data");
            return StatusCode(500, new ScaleResponse 
            { 
                Success = false, 
                Message = $"Internal server error: {ex.Message}" 
            });
        }
    }

    /// <summary>
    /// Close and disconnect all Zebra devices
    /// </summary>
    /// <returns>Operation result</returns>
    [HttpPost("close")]
    public ActionResult<InitializeResponse> Close()
    {
        try
        {
            _logger.LogInformation("Closing Zebra devices...");
            _zebraService.Close();
            
            return Ok(new InitializeResponse 
            { 
                Success = true, 
                Message = "Devices closed successfully" 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing Zebra devices");
            return StatusCode(500, new InitializeResponse 
            { 
                Success = false, 
                Message = $"Internal server error: {ex.Message}" 
            });
        }
    }
}
