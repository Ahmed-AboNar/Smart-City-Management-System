using MongoDB.Bson;
using SmartCity.Domain.Common;

namespace SmartCity.Domain.Entities;

public enum ServiceRequestStatus
{
    Pending,
    Approved,
    Rejected,
    Completed
}

public class ServiceRequest : BaseEntity, IAggregateRoot
{
    public ObjectId ServiceId { get; private set; }
    public ObjectId CitizenId { get; private set; }
    public ServiceRequestStatus Status { get; private set; }
    
    // Using BsonDocument or Dictionary for dynamic form data specific to each service
    public Dictionary<string, object> SubmissionData { get; private set; }
    
    public List<RequestHistory> History { get; private set; } = new();

    private ServiceRequest() { }

    public ServiceRequest(ObjectId serviceId, ObjectId citizenId, Dictionary<string, object> data)
    {
        ServiceId = serviceId;
        CitizenId = citizenId;
        SubmissionData = data;
        Status = ServiceRequestStatus.Pending;
        History.Add(new RequestHistory(ServiceRequestStatus.Pending, "Request Created", citizenId));
    }

    public void UpdateStatus(ServiceRequestStatus newStatus, string remarks, ObjectId changedBy)
    {
        Status = newStatus;
        History.Add(new RequestHistory(newStatus, remarks, changedBy));
    }
}

public class RequestHistory
{
    public ServiceRequestStatus Status { get; private set; }
    public string Remarks { get; private set; }
    public DateTime Timestamp { get; private set; }
    public ObjectId ChangedBy { get; private set; }

    public RequestHistory(ServiceRequestStatus status, string remarks, ObjectId changedBy)
    {
        Status = status;
        Remarks = remarks;
        ChangedBy = changedBy;
        Timestamp = DateTime.UtcNow;
    }
}
