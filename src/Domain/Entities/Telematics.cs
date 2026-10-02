using Domain.Common;

namespace Domain.Entities;

/// <summary>
/// A single telematics reading emitted by a vehicle.
/// This table is append-only: readings are never updated or deleted.
///
/// In a real high-volume system, telematics data would live in a
/// time-series database (TimescaleDB, InfluxDB) or a data lake, not in the
/// same PostgreSQL OLTP database as policies. For this learning project
/// we keep it here but design the table and indexes accordingly.
/// </summary>
public class Telematics: BaseEntity
{   /// <summary>
    /// When the reading was captured by the device.
    /// DateTimeOffset because the reading is tied to a physical place and time.
    /// </summary>
    public DateTimeOffset RecordedAt { get; set; }
    
    /// <summary>
    /// Vehicle speed at the moment of the reading, in km/h.
    /// </summary>
    public decimal SpeedKmh { get; set; }

    /// <summary>
    /// Odometer reading in kilometres, if reported.
    /// Nullable — not all telematics devices report odometer.
    /// </summary>
    public decimal? OdometerKm { get; set; }

    // <summary>
    /// Latitude and longitude, if GPS is available.
    /// Kept as decimals for precision.
    /// </summary>
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    
    /// <summary>
    /// Harsh acceleration event detected in this reading interval.
    /// Boolean flags keep the table simple; a full telematics system would
    /// use a separate event table, but for our scope this is sufficient.
    /// </summary>
    public bool HarshAcceleration { get; set; }
    
    // <summary>
    /// Harsh braking event detected.
    /// </summary>
    public bool HarshBraking { get; set; }
    
    /// <summary>
    /// Harsh cornering event detected.
    /// </summary>
    public bool HarshCornering { get; set; }
    
    
    // --- Relationships ---

    public Guid VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;
    
    
}