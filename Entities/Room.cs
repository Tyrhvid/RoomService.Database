using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace RoomService.Database.Entities;

public class Room
{
    [BsonRepresentation(BsonType.ObjectId), BsonId]
    public string? Id { get; set; }

    public int Name { get; set; }

    public int Price { get; set; }

    public int Size { get; set; }
}