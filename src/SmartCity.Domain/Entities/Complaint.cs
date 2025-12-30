using MongoDB.Bson;
using SmartCity.Domain.Common;

namespace SmartCity.Domain.Entities;

public enum ComplaintStatus
{
    Submitted,
    InProgress,
    Resolved,
    Closed
}

public class Complaint : BaseEntity, IAggregateRoot
{
    public ObjectId CitizenId { get; private set; }
    public string Category { get; private set; }
    public string Description { get; private set; }
    public ComplaintStatus Status { get; private set; }
    public ObjectId? AssignedDepartmentId { get; private set; }

    private Complaint() { }

    public Complaint(ObjectId citizenId, string category, string description)
    {
        CitizenId = citizenId;
        Category = category;
        Description = description;
        Status = ComplaintStatus.Submitted;
    }

    public void AssignToDepartment(ObjectId departmentId)
    {
        AssignedDepartmentId = departmentId;
        Status = ComplaintStatus.InProgress;
    }

    public void Resolve()
    {
        Status = ComplaintStatus.Resolved;
    }
}
