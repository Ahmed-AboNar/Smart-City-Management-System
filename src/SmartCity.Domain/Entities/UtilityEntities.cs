using MongoDB.Bson;
using SmartCity.Domain.Common;

namespace SmartCity.Domain.Entities;

public enum MeterType
{
    Electricity,
    Water
}

public class UtilityMeter : BaseEntity, IAggregateRoot
{
    public string SerialNumber { get; private set; }
    public MeterType Type { get; private set; }
    public ObjectId CitizenId { get; private set; }
    public string Location { get; private set; }
    public double AlertThreshold { get; private set; } // e.g., max usage per hour
    
    public UtilityMeter(string serialNumber, MeterType type, ObjectId citizenId, string location, double alertThreshold)
    {
        SerialNumber = serialNumber;
        Type = type;
        CitizenId = citizenId;
        Location = location;
        AlertThreshold = alertThreshold;
    }
}

public class MeterReading : BaseEntity, IAggregateRoot
{
    public ObjectId MeterId { get; private set; }
    public double Value { get; private set; }
    public DateTime Timestamp { get; private set; }
    
    public MeterReading(ObjectId meterId, double value)
    {
        MeterId = meterId;
        Value = value;
        Timestamp = DateTime.UtcNow;
    }
}
