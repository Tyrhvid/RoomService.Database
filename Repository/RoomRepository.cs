using MongoDB.Bson;
using MongoDB.Driver;
using RoomService.Database.DbContext;
using System.Net.Sockets;
using RoomService.Database.Entities;

namespace RoomService.Database.Repository;

public class RoomRepository : IRoomRepository
{
    private readonly IMongoCollection<Room> _collection;

    public RoomRepository(IMongoDbContext context)
    {
        _collection = context.GetCollection<Room>("Rooms");
    }

    public async Task<Room> GetByIdAsync(string id)
    {
        if (!ObjectId.TryParse(id, out var objectId))
        {
            throw new ArgumentException("Invalid ID format", nameof(id));
        }

        var filter = Builders<Room>.Filter.Eq(r => r.Id, objectId.ToString());
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Room>> GetManyAsync(int start, int count)
    {
        var filter = Builders<Room>.Filter.Empty;
        var rooms = await _collection.Find(filter).Skip(start).Limit(count).ToListAsync();
        return rooms;
    }

    public async Task AddOneAsync(Room item)
    {
        await _collection.InsertOneAsync(item);
    }

    public async Task PutOneAsync(string id, Room item)
    {
        var filter = Builders<Room>.Filter.Eq(r => r.Id, id);

        var update = Builders<Room>.Update
            .Set(r => r.Name, item.Name)
            .Set(r => r.Price, item.Price)
            .Set(r => r.Size, item.Size);

        _collection.UpdateOneAsync(filter, update);
    }


    public async Task DeleteOneAsync(string id)
    {
        var filter = Builders<Room>.Filter.Eq(r => r.Id, id);

        _collection.DeleteOne(filter);
    }
}