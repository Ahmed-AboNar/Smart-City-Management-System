using System.Linq.Expressions;
using MongoDB.Driver;
using SmartCity.Domain.Common;
using SmartCity.Domain.Common.Interfaces;

namespace SmartCity.Infrastructure.Persistence;

/// <summary>
/// Generic Repository implementation for MongoDB.
/// Handles standard CRUD operations for any entity inheriting from BaseEntity.
/// </summary>
/// <typeparam name="T">The Entity type.</typeparam>
public class MongoRepository<T> : IAsyncRepository<T> where T : BaseEntity, IAggregateRoot
{
    private readonly IMongoCollection<T> _collection;

    public MongoRepository(IMongoDatabase database, string collectionName)
    {
        _collection = database.GetCollection<T>(collectionName);
    }

    public async Task<T> GetByIdAsync(string id)
    {
        var objectId = MongoDB.Bson.ObjectId.Parse(id);
        var filter = Builders<T>.Filter.Eq(doc => doc.Id, objectId);
        return await _collection.Find(filter).SingleOrDefaultAsync();
    }

    public async Task<IReadOnlyList<T>> ListAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate)
    {
        return await _collection.Find(predicate).ToListAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        await _collection.InsertOneAsync(entity);
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        var filter = Builders<T>.Filter.Eq(doc => doc.Id, entity.Id);
        await _collection.ReplaceOneAsync(filter, entity);
    }

    public async Task DeleteAsync(T entity)
    {
        var filter = Builders<T>.Filter.Eq(doc => doc.Id, entity.Id);
        await _collection.DeleteOneAsync(filter);
    }
}
