// Add this content to OrderService/Program.cs
using Azure.Messaging.ServiceBus;

string connectionString = "<>";
string queueName = "order-service";

ServiceBusClient client = new ServiceBusClient(connectionString);
ServiceBusSender sender = client.CreateSender(queueName);

var order = new
{
    OrderId = Guid.NewGuid().ToString(),
    CustomerName = "John Doe",
    Address = "123 Baroque Ln, Paris, France",
    Product = "Rolex Day-Date 40"
};

string messageBody = System.Text.Json.JsonSerializer.Serialize(order);
ServiceBusMessage message = new ServiceBusMessage(messageBody);

await sender.SendMessageAsync(message);
Console.WriteLine($"✅ Order sent: {order.OrderId}");

await sender.DisposeAsync();
await client.DisposeAsync();
