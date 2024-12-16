using RoomService.Database;
using RoomService.Database.DbContext;
using RoomService.Database.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDb"));
builder.Services.AddSingleton<IMongoDbContext, MongoDbContext>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddSingleton<RabbitMqService>();
builder.Services.AddHostedService<RabbitMqConsumerService>();

var app = builder.Build();

app.MapGet("/rooms", async (IRoomRepository repo, int start, int count) =>
{
    return await repo.GetManyAsync(start, count);
});

app.Run("http://0.0.0.0:5050");