using Azure.Messaging.EventHubs.Consumer;
using System.Text.Json;

string connectionString = "<>";
string eventHubName = "orders";
string consumerGroup = "invoice-service";

var consumer = new EventHubConsumerClient(EventHubConsumerClient.DefaultConsumerGroupName, connectionString, eventHubName);

Console.WriteLine("🧾 InvoiceService is running...");

await foreach (PartitionEvent ev in consumer.ReadEventsAsync())
{
    var body = ev.Data.EventBody.ToString();
    var order = JsonSerializer.Deserialize<Order>(body);
    Console.WriteLine($"💳 Invoice generated for {order.Customer}: {order.Product} - ${order.Price}");
}

record Order(string OrderId, string Customer, string Address, string Product, int Quantity, decimal Price);
