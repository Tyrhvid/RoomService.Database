using MongoDB.Driver;

namespace RoomService.Database.DbContext;

public interface IMongoDbContext
{
    IMongoCollection<T> GetCollection<T>(string name);
}