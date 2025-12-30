using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SmartCity.Domain.Common;

public abstract class BaseEntity
{
    [BsonId]
    public ObjectId Id { get; protected set; } = ObjectId.GenerateNewId();

    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? CreatedBy { get; protected set; } // UserId
    public DateTime? LastModifiedAt { get; protected set; }
    public DateTime? LastModifiedBy { get; protected set; }
}
