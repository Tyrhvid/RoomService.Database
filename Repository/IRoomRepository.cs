using System.Net.Sockets;
using RoomService.Database.Entities;

namespace RoomService.Database.Repository;

public interface IRoomRepository
{
    Task<Room> GetByIdAsync(string id);
    Task<IEnumerable<Room>> GetManyAsync(int start, int count);
    Task AddOneAsync(Room item);
    Task PutOneAsync(string id, Room item);
    Task DeleteOneAsync(string id);
}