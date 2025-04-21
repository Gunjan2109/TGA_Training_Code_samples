using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using System.Text.Json;

string connectionString = "<>";
string eventHubName = "orders";

var producer = new EventHubProducerClient(connectionString, eventHubName);

var order = new
{
    OrderId = Guid.NewGuid().ToString(),
    Customer = "John Doe",
    Address = "123 Rue Luxe, Paris",
    Product = "Cartier Santos-Dumont",
    Quantity = 1,
    Price = 18500
};

string eventBody = JsonSerializer.Serialize(order);
using EventDataBatch eventBatch = await producer.CreateBatchAsync();
eventBatch.TryAdd(new EventData(eventBody));

await producer.SendAsync(eventBatch);
Console.WriteLine($"📤 Sent order event: {order.OrderId}");

await producer.DisposeAsync();
