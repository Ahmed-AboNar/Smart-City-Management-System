using MongoDB.Bson;
using SmartCity.Domain.Common;

namespace SmartCity.Domain.Entities;

public class ServiceDefinition : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public ObjectId DepartmentId { get; private set; }
    public int SLAInHours { get; private set; }
    public List<string> RequiredDocuments { get; private set; } = new();

    public ServiceDefinition(string name, string description, ObjectId departmentId, int slaInHours, List<string> requiredDocuments)
    {
        Name = name;
        Description = description;
        DepartmentId = departmentId;
        SLAInHours = slaInHours;
        RequiredDocuments = requiredDocuments ?? new List<string>();
    }
}
