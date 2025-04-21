using Azure.Messaging.EventHubs.Consumer;
using System.Text.Json;

string connectionString = "<>";
string eventHubName = "orders";
string consumerGroup = "catalog-service";

var consumer = new EventHubConsumerClient(EventHubConsumerClient.DefaultConsumerGroupName, connectionString, eventHubName);

Console.WriteLine("📦 CatalogService is running...");

await foreach (PartitionEvent ev in consumer.ReadEventsAsync())
{
    var body = ev.Data.EventBody.ToString();
    var order = JsonSerializer.Deserialize<Order>(body);
    Console.WriteLine($"🗃️ Reduced inventory for product: {order.Product} x{order.Quantity}");
}

record Order(string OrderId, string Customer, string Address, string Product, int Quantity, decimal Price);
