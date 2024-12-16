using System.Text;
using System.Text.Json;
using MongoDB.Driver;
using RabbitMQ.Client.Events;
using RoomService.Database.Entities;
using RoomService.Database.Repository;

namespace RoomService.Database;

public class RabbitMqConsumerService : BackgroundService
{
    private readonly RabbitMqService _rabbitMqService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<RabbitMqConsumerService> _logger;

    public RabbitMqConsumerService(RabbitMqService rabbitMqService, IServiceScopeFactory scopeFactory, ILogger<RabbitMqConsumerService> logger)
    {
        _rabbitMqService = rabbitMqService;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_rabbitMqService.Channel);
        consumer.Received += async (model, args) =>
        {
            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IRoomRepository>();

            try
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation("Received RabbitMQ message: {Message}", message);

                var room = JsonSerializer.Deserialize<Room>(message);
                if (room == null)
                {
                    throw new InvalidOperationException("Deserialized room is null or invalid.");
                }

                await repository.AddOneAsync(room);
                _logger.LogInformation("Room successfully processed and saved: {RoomId}", room.Id);

                _rabbitMqService.Acknowledge(args.DeliveryTag);
                _logger.LogInformation("Message acknowledged to RabbitMQ.");
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "JSON deserialization error while processing message: {Message}", Encoding.UTF8.GetString(args.Body.ToArray()));
                // Optionally: Dead Letter or requeue logic for malformed messages
            }
            catch (MongoException mongoEx)
            {
                _logger.LogError(mongoEx, "MongoDB operation failed while saving the room.");
                // Optionally: Implement retry logic if required
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error processing RabbitMQ message.");
            }
        };


        _rabbitMqService.Consume(consumer);
        return Task.CompletedTask;
    }
}